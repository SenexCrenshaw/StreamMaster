import SMPopUp from '@components/sm/SMPopUp';
import React, { useMemo } from 'react';
import StreamGroupFileProfileDataSelector from './StreamGroupFileProfileDataSelector';
import StreamGroupVideoProfileDataSelector from './StreamGroupVideoProfileDataSelector';

export const StreamGroupProfileButton = () => {
  const headerTemplate = useMemo(() => {
    return <>Hey</>;
  }, []);

  return (
    <SMPopUp
      buttonClassName="sm-w-5rem icon-sg"
      buttonLabel="Profiles"
      contentWidthSize="7"
      hasCloseButton={false}
      header={headerTemplate}
      icon="pi-file-edit"
      iconFilled
      info=""
      modalCentered
      title="PROFILES"
    >
      <StreamGroupFileProfileDataSelector />
      <StreamGroupVideoProfileDataSelector />
    </SMPopUp>
  );
};
StreamGroupProfileButton.displayName = 'StreamGroupProfileButton';

export default React.memo(StreamGroupProfileButton);
