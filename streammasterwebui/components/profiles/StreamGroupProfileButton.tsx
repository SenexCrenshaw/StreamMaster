import OutputProfileDataSelector from '@components/profiles/OutputProfileDataSelector';
import SMPopUp from '@components/sm/SMPopUp';
import React from 'react';
import CommandProfileDataSelector from './CommandProfileDataSelector';

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
      title="PROFILES"
      zIndex={11}
    >
      <OutputProfileDataSelector />
      <div className="layout-padding-bottom-lg" />
      <CommandProfileDataSelector />
    </SMPopUp>
  );
};
StreamGroupProfileButton.displayName = 'StreamGroupProfileButton';

export default React.memo(StreamGroupProfileButton);
