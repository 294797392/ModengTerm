#!/bin/bash

# 获取所有网络接口信息
interfaces=$(ifconfig | grep -o '^[a-zA-Z0-9]\+:' | tr -d ':')

# 遍历每个网络接口
for interface in $interfaces; 
do
    # 获取 IP 地址（如果有）
    ip_address=$(ifconfig "$interface" | grep -o 'inet [0-9.]\+' | awk '{print $2}')

    # 获取接收字节数 (RX bytes)
    receive_bytes=$(ifconfig "$interface" | grep 'RX packets' | awk '{print $5}')

    # 获取发送字节数 (TX bytes)
    transmit_bytes=$(ifconfig "$interface" | grep 'TX packets' | awk '{print $5}')

    # 打印结果，每个网卡占用一行
    echo $interface,$ip_address,$receive_bytes,$transmit_bytes
done

