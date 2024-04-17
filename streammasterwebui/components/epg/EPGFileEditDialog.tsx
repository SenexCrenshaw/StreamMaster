import { memo, useState } from 'react';

import { SMCard } from '@components/sm/SMCard';
import EditButton from '@components/buttons/EditButton';
import XButton from '@components/buttons/XButton';

import { UpdateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { EPGFileDto, UpdateEPGFileRequest } from '@lib/smAPI/smapiTypes';
import { Dialog } from 'primereact/dialog';
import EPGFileDialog from './EPGFileDialog';

interface EPGFileEditDialogProperties {
  readonly selectedFile: EPGFileDto;
}

const EPGFileEditDialog = ({ selectedFile }: EPGFileEditDialogProperties) => {
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
        setVisible(false);
      });
  }

  return (
    <>
      <EditButton
        iconFilled={false}
        onClick={(e) => {
          setVisible(true);
        }}
        tooltip="Edit"
      />
      <Dialog
        className="p-0 sm-fileupload-panel default-border"
        visible={visible}
        style={{ top: '-10%', width: '40vw' }}
        onHide={() => setVisible(false)}
        content={({ hide }) => (
          <SMCard
            title="EDIT EPG"
            header={
              <XButton
                iconFilled={false}
                onClick={(e) => {
                  hide(e);
                }}
                tooltip="Close"
              />
            }
          >
            <div className="flex flex-column sm-fileupload p-0 m-0">
              <EPGFileDialog selectedFile={selectedFile} onUpdated={onUpdated} />
            </div>
          </SMCard>
        )}
      ></Dialog>
    </>
  );
};

EPGFileEditDialog.displayName = 'EPGFileEditDialog';

export default memo(EPGFileEditDialog);
