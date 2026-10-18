FROM ros:jazzy-ros-base

SHELL ["/bin/bash", "-o", "pipefail", "-c"]

RUN apt-get update \
    && apt-get install --no-install-recommends -y ros-jazzy-rosbridge-suite \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /opt/wirr-websim
COPY src/wirr_websim_node.py /opt/wirr-websim/wirr_websim_node.py
COPY ros_entrypoint.sh /opt/wirr-websim/ros_entrypoint.sh
RUN chmod +x /opt/wirr-websim/ros_entrypoint.sh \
    && useradd --create-home --uid 10001 websim \
    && chown -R websim:websim /opt/wirr-websim

USER websim
EXPOSE 9090
ENTRYPOINT ["/opt/wirr-websim/ros_entrypoint.sh"]
