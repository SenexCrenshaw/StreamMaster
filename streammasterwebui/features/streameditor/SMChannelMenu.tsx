import SMOverlay from '@components/sm/SMOverlay';
import AutoSetEPGSMChannelDialog from '@components/smchannels/AutoSetEPGSMChannelDialog';
import AddSMChannelsToSGEditor from '@components/smchannels/columns/AddSMChannelsToSGEditor';
import { memo } from 'react';

export interface SChannelMenuProperties {}

const SMChannelMenu = () => {
  // const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  // const isStreamGroupSelected = useMemo(() => {
  //   return selectedStreamGroup !== undefined && selectedStreamGroup.Name !== 'ALL';
  // }, [selectedStreamGroup]);

  return (
    <SMOverlay placement="bottom" icon="pi-bars" iconFilled buttonClassName="icon-orange" contentWidthSize="11rem">
      <div className="sm-channel-menu gap-2">
        {/* <AutoSetSMChannelNumbersDialog disabled={!isStreamGroupSelected} /> */}
        <AutoSetEPGSMChannelDialog menu />
        <AddSMChannelsToSGEditor />
      </div>
    </SMOverlay>
  );
};

SMChannelMenu.displayName = 'SMChannelMenu';

export default memo(SMChannelMenu);
