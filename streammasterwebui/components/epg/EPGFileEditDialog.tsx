import { memo, useRef, useState } from 'react';

import { SMCard } from '@components/SMCard';
import EditButton from '@components/buttons/EditButton';
import XButton from '@components/buttons/XButton';

import { UpdateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { EPGFileDto, UpdateEPGFileRequest } from '@lib/smAPI/smapiTypes';
import { Dialog } from 'primereact/dialog';
import { OverlayPanel } from 'primereact/overlaypanel';
import EPGFileDialog from './EPGFileDialog';

interface EPGFileEditDialogProperties {
  readonly selectedFile: EPGFileDto;
}

const EPGFileEditDialog = ({ selectedFile }: EPGFileEditDialogProperties) => {
  const op = useRef<OverlayPanel>(null);
  const [visible, setVisible] = useState<boolean>(false);

  function onUpdated(request: UpdateEPGFileRequest): void {
    if (request.Id === undefined) {
      return;
    }

    UpdateEPGFile(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        op.current?.hide();
      });
  }

  return (
    <>
      <EditButton
        iconFilled={false}
        onClick={(e) => {
          op.current?.toggle(e);
          setVisible(true);
        }}
        tooltip="Edit"
      />
      {/* <OverlayPanel className="col-5 p-0 sm-fileupload-panel default-border" ref={op} closeOnEscape> */}
      <Dialog
        className="p-0 sm-fileupload-panel default-border"
        visible={visible}
        style={{ top: '-10%', width: '30vw' }}
        onHide={() => setVisible(false)}
        content={
          <SMCard
            title="EDIT EPG"
            header={
              <XButton
                iconFilled={false}
                onClick={(e) => {
                  op.current?.toggle(e);
                  setVisible(false);
                }}
                tooltip="Close"
              />
            }
          >
            <div className="flex flex-column sm-fileupload p-0 m-0">
              <EPGFileDialog selectedFile={selectedFile} onUpdated={onUpdated} />
            </div>
          </SMCard>
        }
      ></Dialog>
      {/* </OverlayPanel> */}
    </>
  );
};

EPGFileEditDialog.displayName = 'EPGFileEditDialog';

export default memo(EPGFileEditDialog);
