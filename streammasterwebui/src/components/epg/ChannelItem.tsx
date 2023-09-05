import React from "react";
import { type Channel } from "planby";
import { ChannelBox, ChannelLogo } from "planby";

type ChannelItemProps = {
  channel: Channel;
}

const ChannelItem = ({ channel }: ChannelItemProps) => {
  const { position, logo } = channel;

  return (
    <ChannelBox
      {...position}
    >

      {/* Overwrite styles by add eg. style={{ maxHeight: 52, maxWidth: 52,... }} */}
      {/* Or stay with default styles */}
      <ChannelLogo
        alt="Logo"
        src={logo}
        style={{ maxHeight: 52, maxWidth: 52 }}
      />

    </ChannelBox>
  );
};

export default React.memo(ChannelItem);
