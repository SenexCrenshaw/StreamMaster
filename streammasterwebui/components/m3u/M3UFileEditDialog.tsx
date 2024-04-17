import { memo, useState } from 'react';

import { SMCard } from '@components/sm/SMCard';
import EditButton from '@components/buttons/EditButton';
import XButton from '@components/buttons/XButton';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import M3UFileDialog from './M3UFileDialog';
import { Dialog } from 'primereact/dialog';

interface M3UFileEditDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileEditDialog = ({ selectedFile }: M3UFileEditDialogProperties) => {
  const [visible, setVisible] = useState<boolean>(false);

  function onUpdated(request: UpdateM3UFileRequest): void {
    if (request.Id === undefined) {
      return;
    }

    UpdateM3UFile(request)
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
      <EditButton iconFilled={false} onClick={(e) => setVisible(true)} tooltip="Edit" />
      <Dialog
        className="p-0 sm-fileupload-panel default-border"
        visible={visible}
        style={{ top: '-10%', width: '40vw' }}
        onHide={() => setVisible(false)}
        content={({ hide }) => (
          <SMCard
            title="EDIT M3U"
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
            <div className="sm-fileupload col-12 p-0 m-0 ">
              <M3UFileDialog selectedFile={selectedFile} onUpdated={onUpdated} />
            </div>
          </SMCard>
        )}
      ></Dialog>
    </>
  );
};

M3UFileEditDialog.displayName = 'M3UFileEditDialog';

export default memo(M3UFileEditDialog);
