import { join } from 'node:path';
import { go, ev, setViewport, screenshot, consoleErrors, close } from './cdp.mjs';

const baseUrl = (process.argv[2] || process.env.SITE_URL || 'http://localhost:5146').replace(/\/+$/, '');
const shotsDir = process.env.SHOTS_DIR;
const sleep = ms => new Promise(r => setTimeout(r, ms));

let passCount = 0;
let failCount = 0;

function ok(page, testName) {
  passCount++;
  console.log(`OK   ${page} — ${testName}`);
}

function fail(page, testName, details = '') {
  failCount++;
  console.log(`FAIL ${page} — ${testName}${details ? ` — ${details}` : ''}`);
}

function warn(page, details) {
  console.log(`WARN ${page} — ${details}`);
}

function isExternalError(err) {
  const externals = [
    'challenges.cloudflare.com',
    'tile.openstreetmap.org',
    'openstreetmap.org',
    'google-analytics.com',
    'googletagmanager.com',
    'fonts.googleapis.com',
    'fonts.gstatic.com'
  ];
  return externals.some(ext => err.includes(ext));
}

async function testPage(pagePath) {
  const fullUrl = `${baseUrl}${pagePath}`;

  // 1. HTTP durumunu fetch ile kontrol et (redirect takip eder)
  try {
    const res = await fetch(fullUrl);
    if (res.status === 200) {
      ok(pagePath, 'HTTP 200');
    } else {
      fail(pagePath, 'HTTP durumu', `Beklenen 200, gelen: ${res.status}`);
    }
  } catch (err) {
    fail(pagePath, 'HTTP isteği', err.message);
  }

  // 2. Masaüstü görünümü (1366x900) ve sayfa açılışı
  await setViewport(1366, 900, false);
  consoleErrors.length = 0;

  try {
    await go(fullUrl);
  } catch (err) {
    fail(pagePath, 'Sayfa yükleme', err.message);
    return;
  }

  // 3. JS konsol ve yakalanmamış istisna kontrolü
  const internalErrors = [];
  const externalErrors = [];
  for (const err of consoleErrors) {
    if (isExternalError(err)) {
      externalErrors.push(err);
    } else {
      internalErrors.push(err);
    }
  }

  if (externalErrors.length > 0) {
    warn(pagePath, `Dış kaynaklı hata (yoksayıldı): ${externalErrors.join('; ')}`);
  }

  if (internalErrors.length === 0) {
    ok(pagePath, 'JS konsol hatası yok');
  } else {
    fail(pagePath, 'JS konsol hatası', internalErrors.slice(0, 3).join(' | '));
  }

  // 4. Lazy görseller için sayfa sonuna kadar kaydır ve bekle
  try {
    await ev('window.scrollTo(0, document.body.scrollHeight)');
    await sleep(800);
    await ev('window.scrollTo(0, 0)');
  } catch {}

  // Aynı kökenli kırık görsel kontrolü
  try {
    const brokenImages = await ev(`
      [...document.images]
        .filter(i => i.complete && i.naturalWidth === 0 && new URL(i.currentSrc || i.src, location.href).origin === location.origin)
        .map(i => i.currentSrc || i.src)
    `) || [];

    if (brokenImages.length === 0) {
      ok(pagePath, 'Kırık görsel yok');
    } else {
      fail(pagePath, 'Kırık görsel', `${brokenImages.length} adet: ${brokenImages.slice(0, 2).join(', ')}`);
    }
  } catch (err) {
    fail(pagePath, 'Kırık görsel kontrolü', err.message);
  }

  // 5. Title ve H1 kontrolü (Compare ve haber-ver hariç tam 1 adet H1)
  try {
    const title = (await ev('document.title') || '').trim();
    const h1Count = await ev('document.querySelectorAll("h1").length') || 0;
    const isCompareOrHaberVer = pagePath === '/Compare' || pagePath === '/haber-ver';

    const titleOk = title.length > 0;
    const h1Ok = isCompareOrHaberVer ? (h1Count >= 0) : (h1Count === 1);

    if (titleOk && h1Ok) {
      ok(pagePath, `Title ve H1 (title: "${title}", h1: ${h1Count})`);
    } else {
      const reasons = [];
      if (!titleOk) reasons.push('title boş');
      if (!h1Ok) reasons.push(`h1 sayısı: ${h1Count} (beklenen: 1)`);
      fail(pagePath, 'Title ve H1', reasons.join(', '));
    }
  } catch (err) {
    fail(pagePath, 'Title ve H1 kontrolü', err.message);
  }

  // 6. Mobil (390x844) görünümde yatay taşma kontrolü
  try {
    await setViewport(390, 844, true);
    await sleep(300);
    const noOverflow = await ev('document.documentElement.scrollWidth <= window.innerWidth + 1');
    if (noOverflow) {
      ok(pagePath, 'Mobil yatay taşma yok');
    } else {
      const info = await ev('JSON.stringify({ scrollWidth: document.documentElement.scrollWidth, innerWidth: window.innerWidth })');
      fail(pagePath, 'Mobil yatay taşma', info);
    }
  } catch (err) {
    fail(pagePath, 'Mobil yatay taşma kontrolü', err.message);
  } finally {
    // Masaüstü boyutuna geri dön
    await setViewport(1366, 900, false);
  }

  // Ekran görüntüsü alma (SHOTS_DIR verilmişse)
  if (shotsDir) {
    try {
      const safeName = (pagePath === '/' ? 'home' : pagePath.replace(/^\//, '').replace(/[^a-zA-Z0-9_-]/g, '_'));
      await screenshot(join(shotsDir, `${safeName}.png`));
    } catch (err) {
      warn(pagePath, `Ekran görüntüsü kaydedilemedi: ${err.message}`);
    }
  }
}

try {
  console.log(`[UI Test] Hedef Adres: ${baseUrl}`);
  if (shotsDir) console.log(`[UI Test] Ekran Görüntüsü Dizini: ${shotsDir}`);

  // Sabit sayfalar
  const staticPages = [
    '/',
    '/satilik',
    '/kiralik',
    '/blog',
    '/Contact',
    '/AboutUs',
    '/kvkk-aydinlatma-metni',
    '/gizlilik-politikasi',
    '/kullanim-kosullari',
    '/haber-ver',
    '/Compare'
  ];

  let listingDetailPath = null;
  let blogDetailPath = null;

  for (const pagePath of staticPages) {
    await testPage(pagePath);

    // /satilik sayfasından ilk ilan detay linkini yakala (3 parçalı ve sonu -<sayı> ile biten)
    if (pagePath === '/satilik' && !listingDetailPath) {
      try {
        listingDetailPath = await ev(`
          (() => {
            const links = Array.from(document.querySelectorAll('a[href^="/satilik/"]'));
            for (const a of links) {
              const href = a.getAttribute('href');
              if (!href) continue;
              const clean = href.split('?')[0].split('#')[0];
              const parts = clean.split('/').filter(Boolean);
              if (parts.length === 3 && /-\\d+$/.test(parts[2])) {
                return clean;
              }
            }
            return null;
          })()
        `);
      } catch {}
    }

    // /blog sayfasından ilk blog detay linkini yakala (/blog/...-<sayı>)
    if (pagePath === '/blog' && !blogDetailPath) {
      try {
        blogDetailPath = await ev(`
          (() => {
            const links = Array.from(document.querySelectorAll('a[href*="/blog/"]'));
            for (const a of links) {
              const href = a.getAttribute('href');
              if (!href) continue;
              const clean = href.split('?')[0].split('#')[0];
              if (/\\/blog\\/[^\\/]+-\\d+$/.test(clean)) {
                return clean;
              }
            }
            return null;
          })()
        `);
      } catch {}
    }
  }

  // Dinamik olarak keşfedilen ilan detay sayfası
  if (listingDetailPath) {
    await testPage(listingDetailPath);
  } else {
    fail('/satilik', 'İlan detay linki keşfi', '3 parçalı ve sonu -<sayı> ile biten ilk href bulunamadı');
  }

  // Dinamik olarak keşfedilen blog detay sayfası
  if (blogDetailPath) {
    await testPage(blogDetailPath);
  } else {
    fail('/blog', 'Blog detay linki keşfi', 'Sonu -<sayı> ile biten ilk blog href bulunamadı');
  }

  // Olmayan bir ilan (404 dönmeli)
  const notFoundPath = '/satilik/konut/olmayan-ilan-999999999';
  try {
    const res = await fetch(`${baseUrl}${notFoundPath}`);
    if (res.status === 404) {
      ok(notFoundPath, 'HTTP 404');
    } else {
      fail(notFoundPath, 'HTTP durumu', `Beklenen 404, gelen: ${res.status}`);
    }
  } catch (err) {
    fail(notFoundPath, 'HTTP isteği', err.message);
  }

  // robots.txt kontrolü (fetch ile 200 ve içerik)
  try {
    const res = await fetch(`${baseUrl}/robots.txt`);
    const text = await res.text();
    if (res.status === 200 && text.trim().length > 0) {
      ok('/robots.txt', 'HTTP 200 ve içerik mevcut');
    } else {
      fail('/robots.txt', 'HTTP durumu / içerik', `Status: ${res.status}`);
    }
  } catch (err) {
    fail('/robots.txt', 'HTTP isteği', err.message);
  }

  // sitemap.xml kontrolü (fetch ile 200, XML ve en az bir <loc>)
  try {
    const res = await fetch(`${baseUrl}/sitemap.xml`);
    const text = await res.text();
    const contentType = res.headers.get('content-type') || '';
    const isXml = contentType.includes('xml') || text.includes('<?xml') || text.includes('<urlset');
    const hasLoc = /<loc>.*?<\/loc>/i.test(text);

    if (res.status === 200 && isXml && hasLoc) {
      ok('/sitemap.xml', 'HTTP 200, XML formatı ve en az bir <loc>');
    } else {
      fail('/sitemap.xml', 'İçerik kontrolü', `Status: ${res.status}, XML: ${isXml}, <loc>: ${hasLoc}`);
    }
  } catch (err) {
    fail('/sitemap.xml', 'HTTP isteği', err.message);
  }

  console.log(`\nPASS ${passCount} FAIL ${failCount}`);
  if (failCount > 0) {
    process.exitCode = 1;
  }
} finally {
  await close();
}
