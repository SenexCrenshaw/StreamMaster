import { memo, useMemo, useRef } from 'react';

import SMButton from '@components/sm/SMButton';

import AutoSetEPGSMChannelDialog from '@components/smchannels/AutoSetEPGSMChannelDialog';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { OverlayPanel } from 'primereact/overlaypanel';
import AutoSetSMChannelNumbersDialog from '../../components/smchannels/AutoSetSMChannelNumbersDialog';

export interface SChannelMenuProperties {}

const SMChannelMenu = () => {
  const op = useRef<OverlayPanel>(null);
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  const isStreamGroupSelected = useMemo(() => {
    return selectedStreamGroup !== undefined && selectedStreamGroup.Name !== 'ALL';
  }, [selectedStreamGroup]);

  return (
    <>
      <OverlayPanel className="sm-overlay" ref={op}>
        <div className="sm-channel-menu">
          <AutoSetSMChannelNumbersDialog disabled={!isStreamGroupSelected} />
          <div className="pt-1"></div>
          <AutoSetEPGSMChannelDialog />
        </div>
      </OverlayPanel>
      <SMButton
        className="icon-orange"
        iconFilled
        icon="pi pi-bars"
        rounded
        onClick={(event) => {
          op.current?.toggle(event);
        }}
        aria-controls="popup_menu_right"
        aria-haspopup
      />
    </>
  );
};

SMChannelMenu.displayName = 'SMChannelMenu';

export default memo(SMChannelMenu);
