#!/bin/bash

# 遍历 /proc 下的所有目录，查找进程目录
for pid in $(ls -d /proc/[0-9]* 2>/dev/null); do
    # 获取进程ID
    pid=${pid##*/}

    # 从 /proc/[pid]/status 文件中提取进程名和内存使用情况
    name=$(grep '^Name:' "/proc/$pid/status" | awk '{print $2}')
    rss=$(grep '^VmRSS:' "/proc/$pid/status" | awk '{print $2}')

    # 从 /proc/[pid]/stat 文件中提取 CPU 使用情况
    stat=($(awk '{print $14, $15}' "/proc/$pid/stat"))
    utime=${stat[0]}  # 用户态 CPU 时间
    ktime=${stat[1]}  # 内核态 CPU 时间

    # 打印结果
	echo $pid,$name,$rss,$utime,$ktime
done
