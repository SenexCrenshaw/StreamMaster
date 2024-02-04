#!/bin/bash

user_name="nonRootUser"
group_name="nonRootGroup"

. /env.sh

# Check if PUID or PGID is set to a non-root value
if [ "$PUID" -ne 0 ]; then
    if getent passwd $PUID > /dev/null 2>&1; then
        user_name=$(getent passwd $PUID | cut -d: -f1)
    else
        adduser --uid $PUID --disabled-password --gecos "nonRootUser" --force-badname "nonRootUser"
    fi
fi

if [ "$PGID" -ne 0 ]; then
    if getent group $PGID > /dev/null 2>&1; then
        group_name=$(getent group $PGID | cut -d: -f1)
    else
        addgroup --gid $PGID --force-badname "nonRootGroup"
    fi
fi

mkdir -p /config/Cache
mkdir -p /config/DB
mkdir -p /config/Logs
mkdir -p /config/PlayLists/EPG
mkdir -p /config/PlayLists/M3U
mkdir -p $PGDATA

# Change ownership of the /app directory
if [ "$PUID" -ne 0 ] || [ "$PGID" -ne 0 ]; then
    echo "Changing ownership of /app to ${PUID:-0}:${PGID:-0}"
    chown -R ${PUID:-0}:${PGID:-0} /app
    echo "Changing ownership of /config to ${PUID:-0}:${PGID:-0}"
    find /config -mindepth 1 -maxdepth 1 -type d -not -path '/config/DB' -exec chown -R ${PUID:-0}:${PGID:-0} {} \;

fi

chown -R postgres:postgres $PGDATA

# Pretty printing the configuration
echo "Configuration:"
echo "--------------"
echo "PGADMIN:"
echo "  Platform Type: $PGADMIN_PLATFORM_TYPE"
echo "  Setup Email: $PGADMIN_SETUP_EMAIL"
echo "  Setup Password: ********" #$PGADMIN_SETUP_PASSWORD"
echo "POSTGRES:"
echo "  User: $POSTGRES_USER"
echo "  Password: ********" #$POSTGRES_PASSWORD"
echo "  DB Name: $POSTGRES_DB"
echo "  Data Directory: $PGDATA"

# Start the database
/usr/local/bin/docker-entrypoint.sh postgres &

#PGADMIN_PLATFORM_TYPE=$PGADMIN_PLATFORM_TYPE PGADMIN_SETUP_EMAIL=$PGADMIN_SETUP_EMAIL PGADMIN_SETUP_PASSWORD=$PGADMIN_SETUP_PASSWORD /usr/pgadmin4/bin/setup-web.sh --yes
# service postgresql start
#service apache2 start

# Execute the main application as the specified user
if [ "$PUID" -ne 0 ] && [ "$PGID" -ne 0 ]; then
    echo "Running as $user_name:$group_name"
    exec gosu $user_name:$group_name "$@"
elif [ "$PUID" -ne 0 ]; then
    echo "Running as $user_name"
    exec gosu $user_name "$@"
elif [ "$PGID" -ne 0 ]; then
    echo "Running as :$group_name"
    exec gosu :$group_name "$@"
else
    echo "Running as root"
    exec "$@"
fi
