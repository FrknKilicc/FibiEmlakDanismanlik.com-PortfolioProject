import { spawn } from 'node:child_process';
import { mkdtempSync, writeFileSync, rmSync, existsSync, mkdirSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { join, dirname } from 'node:path';

// Chrome yolu (ortam değişkeni veya varsayılan Windows yolu)
const chrome = process.env.CHROME_PATH || 'C:/Program Files/Google/Chrome/Application/chrome.exe';
const port = 9500 + Math.floor(Math.random() * 300);
const userDataDir = mkdtempSync(join(tmpdir(), 'cdp-'));

// Headless Chrome sürecini başlat
const proc = spawn(chrome, [
  '--headless=new',
  '--disable-gpu',
  '--ignore-certificate-errors',
  `--remote-debugging-port=${port}`,
  `--user-data-dir=${userDataDir}`,
  'about:blank'
], { stdio: 'ignore' });

const sleep = ms => new Promise(r => setTimeout(r, ms));

// DevTools HTTP uç noktasından sayfa hedefini bul
let target;
for (let i = 0; i < 50 && !target; i++) {
  try {
    const res = await fetch(`http://127.0.0.1:${port}/json`);
    const list = await res.json();
    target = list.find(t => t.type === 'page');
  } catch {}
  if (!target) await sleep(200);
}

if (!target || !target.webSocketDebuggerUrl) {
  try { proc.kill(); } catch {}
  try { rmSync(userDataDir, { recursive: true, force: true }); } catch {}
  throw new Error(`Chrome CDP hedefine bağlanılamadı (port: ${port})`);
}

// WebSocket bağlantısı kur
const ws = new WebSocket(target.webSocketDebuggerUrl);
await new Promise((resolve, reject) => {
  ws.addEventListener('open', resolve, { once: true });
  ws.addEventListener('error', reject, { once: true });
});

let id = 0;
const pend = new Map();
export const consoleErrors = [];
const pageLoadListeners = new Set();

// WebSocket mesajlarını dinle
ws.addEventListener('message', e => {
  let m;
  try {
    m = JSON.parse(e.data);
  } catch {
    return;
  }

  if (m.id && pend.has(m.id)) {
    pend.get(m.id)(m);
    pend.delete(m.id);
  }

  if (m.method === 'Page.loadEventFired') {
    for (const listener of pageLoadListeners) {
      try { listener(); } catch {}
    }
  }

  if (m.method === 'Runtime.exceptionThrown') {
    const details = m.params?.exceptionDetails;
    const desc = (details?.exception?.description || '').split('\n').slice(0, 2).join(' ');
    consoleErrors.push(`${details?.text || 'Exception'} ${desc}`.trim());
  }

  if (m.method === 'Log.entryAdded' && m.params?.entry?.level === 'error') {
    const entry = m.params.entry;
    consoleErrors.push(`${entry.text || ''} ${entry.url || ''}`.trim());
  }
});

// CDP komutu gönder
export const send = (method, params = {}) => new Promise(resolve => {
  const i = ++id;
  pend.set(i, resolve);
  ws.send(JSON.stringify({ id: i, method, params }));
});

// Sayfada JavaScript kodu çalıştır ve değerini dön
export const ev = async expr => {
  const res = await send('Runtime.evaluate', {
    expression: expr,
    returnByValue: true,
    awaitPromise: true
  });
  return res.result?.result?.value;
};

// URL'e git, Page.loadEventFired bekle (en fazla 20 sn), sonra 500 ms bekle
export const go = async url => {
  const loadPromise = new Promise(resolve => {
    const onLoaded = () => {
      pageLoadListeners.delete(onLoaded);
      resolve();
    };
    pageLoadListeners.add(onLoaded);
  });

  await send('Page.navigate', { url });
  await Promise.race([loadPromise, sleep(20000)]);
  await sleep(500);
};

// Viewport boyutunu ve mobil modunu ayarla
export const setViewport = async (w, h, mobile = false) => {
  await send('Emulation.setDeviceMetricsOverride', {
    width: w,
    height: h,
    deviceScaleFactor: 1,
    mobile: Boolean(mobile)
  });
};

// Ekran görüntüsü al ve dosyaya yaz
export const screenshot = async path => {
  const res = await send('Page.captureScreenshot', { format: 'png' });
  const data = res.result?.data;
  if (data) {
    const dir = dirname(path);
    if (dir && !existsSync(dir)) {
      mkdirSync(dir, { recursive: true });
    }
    writeFileSync(path, Buffer.from(data, 'base64'));
  }
};

// Tarayıcı oturumunu ve geçici profili temizle
export const close = async () => {
  try {
    ws.close();
  } catch {}
  try {
    proc.kill();
  } catch {}
  await sleep(300);
  try {
    rmSync(userDataDir, { recursive: true, force: true });
  } catch {}
};

// CDP alanlarını etkinleştir
await send('Page.enable');
await send('Runtime.enable');
await send('Log.enable');

export default {
  send,
  ev,
  go,
  setViewport,
  screenshot,
  consoleErrors,
  close
};
