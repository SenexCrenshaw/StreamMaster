import { memo } from 'react';

import BaseButton from '@components/buttons/BaseButton';
import { ConfirmPopup, confirmPopup } from 'primereact/confirmpopup';

import AutoSetSMChannelNumbersDialog from './AutoSetSMChannelNumbersDialog';

export interface SChannelMenuProperties {}

const SMChannelMenu = () => {
  const accept = () => {};

  const reject = () => {};

  const confirm1 = (event: any) => {
    confirmPopup({
      accept,
      reject,
      target: event.currentTarget
    });
  };

  return (
    <>
      <ConfirmPopup
        content={({ message, acceptBtnRef, rejectBtnRef, hide }) => (
          <div className="sm-channel-menu">
            <AutoSetSMChannelNumbersDialog label="Auto Number" />
          </div>
        )}
      />
      <BaseButton
        className="button-orange"
        icon="pi pi-bars"
        rounded
        onClick={(event) => {
          confirm1(event);
        }}
        aria-controls="popup_menu_right"
        aria-haspopup
      />
    </>
  );
};

SMChannelMenu.displayName = 'SMChannelMenu';

export default memo(SMChannelMenu);
