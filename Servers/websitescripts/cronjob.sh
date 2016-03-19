#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

> /etc/cron.hourly/purgemysqllogs
chmod +x /etc/cron.hourly/purgemysqllogs
echo "#!/usr/bin/env bash" >> /etc/cron.hourly/purgemysqllogs
echo "set -e" >> /etc/cron.hourly/purgemysqllogs
echo "set -u" >> /etc/cron.hourly/purgemysqllogs
echo "# Purge all mariadb logs that are taking up all the space" >> /etc/cron.hourly/purgemysqllogs
echo "mysql -u \"root\" -p\"Password123\" -e \"PURGE BINARY LOGS BEFORE DATE_SUB( NOW( ), INTERVAL 7 DAY);\"" >> /etc/cron.hourly/purgemysqllogs
echo "" >> /etc/cron.hourly/purgemysqllogs

