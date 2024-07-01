import SMOverlay from '@components/sm/SMOverlay';
import AutoSetEPGSMChannelDialog from '@components/smchannels/AutoSetEPGSMChannelDialog';
import AutoSetSMChannelNumbersDialog from '@components/smchannels/AutoSetSMChannelNumbersDialog';
import SMChannelMultiVisibleDialog from '@components/smchannels/SMChannelMultiVisibleDialog';
import AddSMChannelsToSGEditor from '@components/smchannels/columns/AddSMChannelsToSGEditor';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { memo } from 'react';

export interface SChannelMenuProperties {}

const SMChannelMenu = () => {
  const { isTrue: smTableIsSimple } = useIsTrue('isSimple');
  const selectedItemsKey = 'selectSelectedSMChannelDtoItems';

  return (
    <SMOverlay placement={smTableIsSimple ? 'bottom-end' : 'bottom'} icon="pi-bars" iconFilled buttonClassName="icon-orange" contentWidthSize="11rem">
      <div className="sm-channel-menu gap-2">
        <AutoSetEPGSMChannelDialog menu />
        <AutoSetSMChannelNumbersDialog />
        <AddSMChannelsToSGEditor />
        <SMChannelMultiVisibleDialog selectedItemsKey={selectedItemsKey} menu />
      </div>
    </SMOverlay>
  );
};

SMChannelMenu.displayName = 'SMChannelMenu';

export default memo(SMChannelMenu);
