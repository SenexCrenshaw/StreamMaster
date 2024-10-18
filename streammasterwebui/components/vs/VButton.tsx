import SMPopUp from '@components/sm/SMPopUp';
import { StreamGroupDto, StreamGroupProfile } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';
import VDataSelector from './VDataSelector';

interface VButtonProperties {
  streamGroupDto: StreamGroupDto | null;
  streamGroupProfile?: StreamGroupProfile | null;
}

const VButton = ({ streamGroupDto, streamGroupProfile }: VButtonProperties) => {
  return (
    <SMPopUp
      buttonClassName="icon-blue"
      contentWidthSize="6"
      icon="pi-list-check"
      modal
      placement="bottom-end"
      title="Short Urls"
      tooltip="Short Urls"
      zIndex={12}
    >
      <VDataSelector streamGroupDto={streamGroupDto} streamGroupProfile={streamGroupProfile} />
    </SMPopUp>
  );
};

VButton.displayName = 'VButton';

export interface M3UFilesEditorProperties {}

export default memo(VButton);
