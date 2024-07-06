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

  // const mainSM = {
  //   animateOn: 'hover' as 'hover',
  //   direction: 'bottom' as Direction,
  //   icon: '/images/sm_logo.png',
  //   modal: true,
  //   shape: 'line' as Shape
  // };

  // const smItems = [
  //   {
  //     template: <AutoSetEPGSMChannelDialog menu />
  //   },
  //   {
  //     template: <AutoSetSMChannelNumbersDialog />
  //   },
  //   {
  //     template: <AddSMChannelsToSGEditor />
  //   },
  //   {
  //     template: <SMChannelMultiVisibleDialog selectedItemsKey={selectedItemsKey} menu />
  //   }
  // ];

  return (
    <>
      <SMOverlay placement={smTableIsSimple ? 'bottom-end' : 'bottom'} icon="pi-bars" iconFilled buttonClassName="icon-orange" contentWidthSize="11rem">
        <div className="sm-channel-menu gap-2">
          <AutoSetEPGSMChannelDialog menu />
          <AutoSetSMChannelNumbersDialog selectedItemsKey={selectedItemsKey} />
          <AddSMChannelsToSGEditor />
          <SMChannelMultiVisibleDialog selectedItemsKey={selectedItemsKey} menu />
        </div>
      </SMOverlay>
      {/* <SMSpeedMenu items={smItems} mainItem={mainSM} backgroundWidth="300px" /> */}
    </>
  );
};

SMChannelMenu.displayName = 'SMChannelMenu';

export default memo(SMChannelMenu);
