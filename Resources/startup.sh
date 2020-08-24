#!/bin/sh

case "$1" in
    "start")
        cd /mnt/user
        if [ $? ] # if cd was succesful
        then
            ./eth-can-router
        else
            echo "Failed to find /mnt/user directory" && exit 1
        fi
    ;;
    "stop")
        killall -9 eth-can-router
    ;;
    
    "restart"|"reload")
        "$0" stop
        "$0" start
    ;;
    *)
        echo "Usage: $0 {start|stop|restart}"
        exit 1
esac

exit $?
