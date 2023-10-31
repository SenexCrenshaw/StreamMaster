#!/bin/bash

# Default values for PUID and PGID
PUID=${PUID:-1000}
PGID=${PGID:-1000}

user_name="nonRootUser"
group_name="nonRootGroup"

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

# Change ownership of the /app directory
if [ "$PUID" -ne 0 ] || [ "$PGID" -ne 0 ]; then
    echo "Changing ownership of /app to ${PUID}:${PGID}"
    chown -R ${PUID}:${PGID} /app
fi

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
