#!/usr/bin/env bash
# Fibi Emlak: Ubuntu 24.04 VDS'i sıfırdan hazırlar. Bir kez, root olarak çalıştırılır:
#   sudo bash deploy/server/setup.sh
# Tekrar çalıştırmak güvenlidir: mevcut şifreleri, ayar dosyalarını ve veritabanını ezmez.
#
# Kurulanlar: .NET 8 çalışma ortamı, SQL Server 2025 Express (yalnızca 127.0.0.1), nginx, certbot (Let's Encrypt),
# ufw güvenlik duvarı (yalnızca 22/80/443), fail2ban, otomatik güvenlik güncellemeleri, gece yedeği, 2 GB swap.
set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SA_PASSWORD_FILE=/root/.fibi-sa-password
CI_KEY=/root/fibi-ci-key

log() { echo -e "\n\033[1;32m==> $*\033[0m"; }
random_secret() { openssl rand -hex "$1"; }

[[ $EUID -eq 0 ]] || { echo "root olarak çalıştır: sudo bash $0" >&2; exit 1; }
. /etc/os-release
[[ "$ID" == "ubuntu" && "$VERSION_ID" == "24.04" ]] || { echo "Bu betik Ubuntu 24.04 içindir (bulunan: $PRETTY_NAME)." >&2; exit 1; }

log "Saat dilimi ve sistem güncellemesi"
timedatectl set-timezone Europe/Istanbul
export DEBIAN_FRONTEND=noninteractive
apt-get update
apt-get -y upgrade
apt-get install -y curl gnupg ca-certificates openssl ufw fail2ban unattended-upgrades \
    nginx certbot python3-certbot-nginx aspnetcore-runtime-8.0 rclone

log "Swap (bellek yetmezse sistem çökmesin diye 2 GB)"
if ! swapon --show | grep -q .; then
    fallocate -l 2G /swapfile
    chmod 600 /swapfile
    mkswap /swapfile
    swapon /swapfile
    grep -q '^/swapfile' /etc/fstab || echo '/swapfile none swap sw 0 0' >> /etc/fstab
    sysctl -w vm.swappiness=10
    echo 'vm.swappiness=10' > /etc/sysctl.d/99-fibi-swap.conf
fi

log "SQL Server 2025 Express"
if [[ ! -f /usr/share/keyrings/microsoft-prod.gpg ]]; then
    curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg
fi
curl -fsSL https://packages.microsoft.com/config/ubuntu/24.04/mssql-server-2025.list -o /etc/apt/sources.list.d/mssql-server-2025.list
curl -fsSL https://packages.microsoft.com/config/ubuntu/24.04/prod.list -o /etc/apt/sources.list.d/microsoft-prod.list
apt-get update
apt-get install -y mssql-server
ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev

if [[ ! -f "$SA_PASSWORD_FILE" ]]; then
    # SQL Server şifre kuralı: büyük + küçük harf + rakam + sembol
    echo "Fb1!$(random_secret 16)" > "$SA_PASSWORD_FILE"
    chmod 600 "$SA_PASSWORD_FILE"
    systemctl stop mssql-server || true
    MSSQL_SA_PASSWORD="$(cat "$SA_PASSWORD_FILE")" MSSQL_PID=Express ACCEPT_EULA=Y /opt/mssql/bin/mssql-conf -n setup
fi
# Yalnızca bu sunucudan bağlanılabilir; 1433 dışarıya hiç açılmaz
/opt/mssql/bin/mssql-conf set network.ipaddress 127.0.0.1
/opt/mssql/bin/mssql-conf set memory.memorylimitmb 2048
systemctl enable mssql-server
systemctl restart mssql-server

log "Kullanıcılar"
id fibi >/dev/null 2>&1 || useradd --system --home-dir /var/lib/fibi --shell /usr/sbin/nologin fibi
id deploy >/dev/null 2>&1 || useradd --create-home --shell /bin/bash deploy
passwd -l deploy >/dev/null

log "Klasörler"
mkdir -p /var/www/fibi/{api,site,panel}/releases \
         /var/lib/fibi/uploads /var/lib/fibi/dp-keys/{api,panel} \
         /var/log/fibi /var/backups/fibi/{db,files} /etc/fibi \
         /usr/local/share/fibi/db /home/deploy/incoming
chown -R fibi:fibi /var/lib/fibi /var/log/fibi
chmod 750 /var/lib/fibi /var/log/fibi
chmod 700 /var/lib/fibi/dp-keys /var/lib/fibi/dp-keys/{api,panel}
chown mssql:mssql /var/backups/fibi/db
chmod 750 /var/backups/fibi/db
chmod 700 /var/backups/fibi/files
chown deploy:deploy /home/deploy/incoming

log "Ayar dosyaları (/etc/fibi) — mevcutlar korunur"
APP_PASSWORD="Fb1!$(random_secret 16)"
MIGRATOR_PASSWORD="Fb1!$(random_secret 16)"
if [[ ! -f /etc/fibi/api.env ]]; then
    sed -e "s|<UYGULAMA_SIFRESI>|$APP_PASSWORD|" \
        -e "s|<JWT_ANAHTARI>|$(random_secret 48)|" \
        "$HERE/env/api.env.example" > /etc/fibi/api.env
fi
[[ -f /etc/fibi/site.env ]]  || cp "$HERE/env/site.env.example"  /etc/fibi/site.env
[[ -f /etc/fibi/panel.env ]] || cp "$HERE/env/panel.env.example" /etc/fibi/panel.env
if [[ ! -f /etc/fibi/migrator.env ]]; then
    sed -e "s|<MIGRATOR_SIFRESI>|$MIGRATOR_PASSWORD|" "$HERE/env/migrator.env.example" > /etc/fibi/migrator.env
fi
chown root:fibi /etc/fibi/{api,site,panel}.env
chmod 640 /etc/fibi/{api,site,panel}.env
chown root:root /etc/fibi/migrator.env
chmod 600 /etc/fibi/migrator.env
chmod 750 /etc/fibi
chown root:fibi /etc/fibi

log "Betikler, servisler, nginx"
install -m 750 -o root -g root "$HERE"/scripts/fibi-* /usr/local/bin/
install -m 640 -o root -g root "$HERE"/db/*.sql /usr/local/share/fibi/db/
install -m 644 "$HERE"/systemd/fibi-*.service /etc/systemd/system/
systemctl daemon-reload
systemctl enable fibi-api fibi-site fibi-panel

install -m 644 "$HERE/nginx/fibi.conf" /etc/nginx/sites-available/fibi.conf
ln -sfn /etc/nginx/sites-available/fibi.conf /etc/nginx/sites-enabled/fibi.conf
rm -f /etc/nginx/sites-enabled/default
nginx -t
systemctl reload nginx

# deploy kullanıcısı yalnızca fibi-deploy betiğini root olarak çalıştırabilir, başka hiçbir şeyi değil
cat > /etc/sudoers.d/fibi-deploy <<'EOF'
deploy ALL=(root) NOPASSWD: /usr/local/bin/fibi-deploy
EOF
chmod 440 /etc/sudoers.d/fibi-deploy
visudo -cf /etc/sudoers.d/fibi-deploy

log "GitHub Actions için deploy anahtarı"
install -d -m 700 -o deploy -g deploy /home/deploy/.ssh
if [[ ! -f "$CI_KEY" ]] && ! grep -q 'fibi-github-actions' /home/deploy/.ssh/authorized_keys 2>/dev/null; then
    ssh-keygen -t ed25519 -N '' -C 'fibi-github-actions' -f "$CI_KEY" >/dev/null
    # Anahtar yalnızca dosya kopyalama ve fibi-deploy için kullanılabilir (port yönlendirme vb. kapalı)
    echo "no-port-forwarding,no-X11-forwarding,no-agent-forwarding $(cat "$CI_KEY.pub")" >> /home/deploy/.ssh/authorized_keys
fi
chown deploy:deploy /home/deploy/.ssh/authorized_keys
chmod 600 /home/deploy/.ssh/authorized_keys

log "Güvenlik duvarı ve saldırı koruması"
ufw default deny incoming
ufw default allow outgoing
ufw allow OpenSSH
ufw allow 'Nginx Full'
ufw --force enable
cat > /etc/fail2ban/jail.d/fibi.local <<'EOF'
[sshd]
enabled = true
maxretry = 5
findtime = 10m
bantime = 1h
EOF
systemctl enable --now fail2ban
systemctl restart fail2ban
dpkg-reconfigure -f noninteractive unattended-upgrades

log "Gece yedeği (her gece 03:30)"
cat > /etc/cron.d/fibi-backup <<'EOF'
30 3 * * * root /usr/local/bin/fibi-backup >> /var/log/fibi/backup.log 2>&1
EOF
chmod 644 /etc/cron.d/fibi-backup

log "KURULUM TAMAM"
echo "Sıradaki adımlar (rehberdeki sırayla):"
echo "  1) Turnstile anahtarlarını gir:  sudo nano /etc/fibi/site.env  ve  /etc/fibi/panel.env  ve  /etc/fibi/api.env"
if [[ -f "$CI_KEY" ]]; then
    echo "  2) Aşağıdaki ÖZEL anahtarı GitHub > Settings > Secrets > Actions > DEPLOY_SSH_KEY olarak kaydet,"
    echo "     sonra bu dosyayı sil:  sudo shred -u $CI_KEY $CI_KEY.pub"
    echo "----------------------------------------------------------------"
    cat "$CI_KEY"
    echo "----------------------------------------------------------------"
fi
echo "  3) Sunucu SSH parmak izi (DEPLOY_KNOWN_HOSTS secret'ı):"
ssh-keyscan -t ed25519 127.0.0.1 2>/dev/null | sed "s/^127.0.0.1/$(curl -fsS -4 https://ifconfig.me 2>/dev/null || echo SUNUCU_IP)/"
