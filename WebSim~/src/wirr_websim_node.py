#!/usr/bin/env python3
"""Deterministic ROS 2 signal generator for WiRR Lab 06.

The public contract intentionally matches Runtime/WebSimStateSource.cs.  A
session is created lazily after Unity publishes a JSON control message to
/wirr/control; its JointState and status topics are then isolated by session.
"""

from __future__ import annotations

import json
import math
import os
import re
from dataclasses import dataclass, field
from typing import Dict, List

import rclpy
from rclpy.node import Node
from rosgraph_msgs.msg import Clock
from sensor_msgs.msg import JointState
from std_msgs.msg import String

SESSION_RE = re.compile(r"^[A-Z0-9_-]{3,12}$")
VALID_ROBOTS = {"rrbot": 2, "wirr-arm3": 3}
MOTIONS = {
    "A": (0.70, -0.55, 0.35),
    "B": (-0.85, 0.60, -0.45),
    "C": (1.10, 0.25, 0.75),
}


@dataclass
class Session:
    code: str
    robot: str
    joint_count: int
    state_publisher: object
    status_publisher: object
    targets: List[float] = field(default_factory=list)
    positions: List[float] = field(default_factory=list)
    command_subscription: object = None


class WiRRWebSim(Node):
    def __init__(self) -> None:
        super().__init__("wirr_websim")
        self._sessions: Dict[str, Session] = {}
        self._status = self.create_publisher(String, "/wirr/status", 10)
        self.create_subscription(String, "/wirr/control", self._on_control, 20)
        self._clock_publisher = self.create_publisher(Clock, "/clock", 10)
        hz = max(1.0, float(os.getenv("WIRR_PUBLISH_HZ", "30")))
        self._dt = 1.0 / hz
        self.create_timer(self._dt, self._publish)
        self._broadcast("READY: WiRR WebSim ROS 2 signal generator")

    def _broadcast(self, text: str) -> None:
        message = String(data=text)
        self._status.publish(message)
        self.get_logger().info(text)

    def _session_status(self, session: Session, text: str) -> None:
        session.status_publisher.publish(String(data=text))
        self._broadcast(f"{session.code}: {text}")

    @staticmethod
    def _parse_control(raw: str) -> dict | None:
        try:
            value = json.loads(raw)
        except json.JSONDecodeError:
            return None
        return value if isinstance(value, dict) else None

    def _on_control(self, message: String) -> None:
        payload = self._parse_control(message.data)
        if payload is None:
            self._broadcast("ERROR: invalid /wirr/control JSON")
            return
        code = str(payload.get("session", "")).upper()
        robot = str(payload.get("robot", "rrbot"))
        action = str(payload.get("action", "")).lower()
        if not SESSION_RE.fullmatch(code):
            self._broadcast("ERROR: session must match [A-Z0-9_-]{3,12}")
            return
        if action == "create":
            if robot not in VALID_ROBOTS:
                self._broadcast(f"ERROR: unsupported robot '{robot}'")
                return
            session = self._sessions.get(code)
            if session is None:
                self._create_session(code, robot)
            elif session.robot != robot:
                self._session_status(session, f"ERROR: session already uses {session.robot}")
            else:
                self._session_status(session, "LIVE: session already active")
        elif action == "destroy":
            self._destroy_session(code)
        elif action == "ping":
            session = self._sessions.get(code)
            if session is None:
                self._broadcast(f"ERROR: unknown session {code}; send action=create first")
            else:
                self._session_status(session, "LIVE: pong")
        else:
            self._broadcast(f"ERROR: unsupported action '{action}'")

    def _create_session(self, code: str, robot: str) -> None:
        count = VALID_ROBOTS[robot]
        session = Session(
            code=code,
            robot=robot,
            joint_count=count,
            state_publisher=self.create_publisher(JointState, f"/wirr/{code}/joint_states", 10),
            status_publisher=self.create_publisher(String, f"/wirr/{code}/status", 10),
            targets=[0.0] * count,
            positions=[0.0] * count,
        )
        session.command_subscription = self.create_subscription(
            String, f"/wirr/{code}/command", lambda msg, key=code: self._on_command(key, msg), 10
        )
        self._sessions[code] = session
        self._session_status(session, f"LIVE: created {robot} ({count} joints)")

    def _destroy_session(self, code: str) -> None:
        session = self._sessions.pop(code, None)
        if session is None:
            self._broadcast(f"ERROR: unknown session {code}")
            return
        self.destroy_subscription(session.command_subscription)
        self.destroy_publisher(session.state_publisher)
        self.destroy_publisher(session.status_publisher)
        self._broadcast(f"{code}: DESTROYED")

    def _on_command(self, code: str, message: String) -> None:
        session = self._sessions.get(code)
        if session is None:
            return
        command = message.data.strip().lower()
        if command in {"home", "reset"}:
            session.targets = [0.0] * session.joint_count
            if command == "reset":
                session.positions = [0.0] * session.joint_count
            self._session_status(session, f"LIVE: {command}")
            return
        if command.startswith("motion:"):
            motion = command.partition(":")[2].upper()
            target = MOTIONS.get(motion)
            if target is None:
                self._session_status(session, f"ERROR: unknown motion '{motion}'")
                return
            session.targets = list(target[: session.joint_count])
            self._session_status(session, f"LIVE: motion {motion}")
            return
        self._session_status(session, f"ERROR: unknown command '{message.data}'")

    def _publish(self) -> None:
        now = self.get_clock().now().to_msg()
        self._clock_publisher.publish(Clock(clock=now))
        # Critically damped-like first-order motion: observable but repeatable.
        alpha = 1.0 - math.exp(-6.0 * self._dt)
        for session in self._sessions.values():
            session.positions = [
                current + alpha * (target - current)
                for current, target in zip(session.positions, session.targets)
            ]
            state = JointState()
            state.header.stamp = now
            state.name = [f"joint{i + 1}" for i in range(session.joint_count)]
            state.position = session.positions
            state.velocity = [0.0] * session.joint_count
            session.state_publisher.publish(state)


def main() -> None:
    rclpy.init()
    node = WiRRWebSim()
    try:
        rclpy.spin(node)
    except KeyboardInterrupt:
        pass
    finally:
        node.destroy_node()
        rclpy.shutdown()


if __name__ == "__main__":
    main()
