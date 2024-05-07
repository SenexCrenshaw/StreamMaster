import { memo, useRef } from 'react';

import SMButton from '@components/sm/SMButton';

import { OverlayPanel } from 'primereact/overlaypanel';
import AutoSetSMChannelNumbersDialog from './AutoSetSMChannelNumbersDialog';

export interface SChannelMenuProperties {}

const SMChannelMenu = () => {
  const op = useRef<OverlayPanel>(null);

  return (
    <>
      <OverlayPanel className="sm-overlay" ref={op}>
        <div className="sm-channel-menu">
          <AutoSetSMChannelNumbersDialog />
        </div>
      </OverlayPanel>
      <SMButton
        className="icon-orange-filled"
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
