#!/usr/bin/env bash
# Skrypty setup ROS 2 czytają opcjonalne zmienne środowiskowe. Włączamy `nounset`
# dopiero po ich załadowaniu, aby kontener działał także w czystym środowisku.
set -eo pipefail
source /opt/ros/jazzy/setup.bash
set -u

# rosbridge exposes ROS 2 topics to the Unity WebSocket client.  The node below
# owns only the deterministic laboratory model; it can later be replaced by a
# Gazebo publisher without changing the Unity protocol.
ros2 launch rosbridge_server rosbridge_websocket_launch.xml port:=9090 &
bridge_pid=$!

cleanup() {
  kill "$bridge_pid" 2>/dev/null || true
  wait "$bridge_pid" 2>/dev/null || true
}
trap cleanup EXIT INT TERM

python3 /opt/wirr-websim/wirr_websim_node.py
