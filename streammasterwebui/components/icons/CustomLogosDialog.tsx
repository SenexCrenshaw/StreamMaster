import SMPopUp from '@components/sm/SMPopUp';
import React, { useMemo } from 'react';
import { CustomLogosAddDialog } from './CustomLogosAddDialog';
import { CustomLogosDataSelector } from './CustomLogosDataSelector';

export function CustomLogosDialog(): React.ReactElement {
  const headerRight = useMemo((): React.ReactNode => {
    return (
      <div className="flex w-12 gap-1 justify-content-end align-content-center">
        <CustomLogosAddDialog />
      </div>
    );
  }, []);

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      contentWidthSize="5"
      header={headerRight}
      icon="pi-pencil"
      iconFilled
      info=""
      label="Custom Logos"
      modal
      modalCentered
      noCloseButton={false}
      placement="bottom-end"
      title="CUSTOM LOGOS"
      zIndex={11}
    >
      <CustomLogosDataSelector />
    </SMPopUp>
  );
}
