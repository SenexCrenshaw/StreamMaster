import SMPopUp from '@components/sm/SMPopUp';
import StreamGroupOutputProfileDataSelector from '@components/streamGroup/profiles/StreamGroupOutputProfileDataSelector';
import StreamGroupVideoProfileDataSelector from '@components/streamGroup/profiles/StreamGroupVideoProfileDataSelector';
import React from 'react';

export const StreamGroupProfileButton = () => {
  return (
    <SMPopUp
      buttonClassName="sm-w-5rem icon-orange"
      buttonLabel="Profiles"
      contentWidthSize="6"
      modal
      modalClosable
      showClose={false}
      icon="pi-briefcase"
      iconFilled
      info=""
      modalCentered
      noBorderChildren
      showRemember={false}
      title="PROFILES"
      zIndex={11}
    >
      <StreamGroupOutputProfileDataSelector />
      <div className="layout-padding-bottom-lg" />
      <StreamGroupVideoProfileDataSelector />
    </SMPopUp>
  );
};
StreamGroupProfileButton.displayName = 'StreamGroupProfileButton';

export default React.memo(StreamGroupProfileButton);
