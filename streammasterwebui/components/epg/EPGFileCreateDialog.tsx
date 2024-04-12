import React, { useCallback, useRef, useState } from 'react';

import AddButton from '@components/buttons/AddButton';
import { FileUpload } from 'primereact/fileupload';

import { SMCard } from '@components/sm/SMCard';
import XButton from '@components/buttons/XButton';
import SMFileUpload from '@components/file/SMFileUpload';

import { CreateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { CreateEPGFileRequest, EPGFileDto } from '@lib/smAPI/smapiTypes';
import { Dialog } from 'primereact/dialog';
import { OverlayPanel } from 'primereact/overlaypanel';
import EPGFileDialog from './EPGFileDialog';

export interface EPGFileCreateDialogProperties {
  readonly infoMessage?: string;
  readonly onHide?: (didUpload: boolean) => void;
  readonly onUploadComplete: () => void;
  readonly show?: boolean | null;
  readonly showButton?: boolean | null;
}

export const EPGFileCreateDialog = ({ onHide, onUploadComplete, showButton }: EPGFileCreateDialogProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const op = useRef<OverlayPanel>(null);
  const [visible, setVisible] = useState<boolean>(false);

  const defaultValues = {
    Color: '',
    EPGNumber: 1,
    HoursToUpdate: 72,
    Name: '',
    TimeShift: 0
  } as EPGFileDto;

  const [epgFileDto, setEPGFileDto] = React.useState<EPGFileDto>(defaultValues);

  const ReturnToParent = useCallback(
    (didUpload?: boolean) => {
      if (fileUploadReference.current) {
        fileUploadReference.current.clear();
      }

      onHide?.(didUpload ?? false);
      onUploadComplete();
    },
    [onHide, onUploadComplete]
  );

  const onCreateFromSource = useCallback(
    async (source: string) => {
      const request = {} as CreateEPGFileRequest;
      request.Name = epgFileDto.Name;
      await CreateEPGFile(request)
        .then(() => {})
        .catch((error) => {
          console.error('Error uploading M3U', error);
        })
        .finally(() => {
          ReturnToParent(true);
        });
    },
    [ReturnToParent, epgFileDto]
  );

  const setName = (value: string) => {
    if (epgFileDto && epgFileDto.Name !== value) {
      const m3uFileDtoCopy = { ...epgFileDto };
      m3uFileDtoCopy.Name = value;
      console.log('M3UFileCreateDialog setName', value);
      setEPGFileDto(m3uFileDtoCopy);
    }
  };

  return (
    <>
      {/* <OverlayPanel className="col-5 p-0 sm-fileupload-panel default-border" ref={op} showCloseIcon={false} closeOnEscape> */}
      <Dialog header="Header" visible={visible} style={{ width: '50vw' }} onHide={() => setVisible(false)}>
        <SMCard title="ADD M3U" header={<XButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} tooltip="Close" />}>
          <div className="sm-fileupload col-12 p-0 m-0 ">
            <div className="px-2">
              <SMFileUpload
                epgFileDto={epgFileDto}
                onCreateFromSource={onCreateFromSource}
                onUploadComplete={() => {
                  ReturnToParent(true);
                }}
                onName={(name) => {
                  setName(name);
                }}
              />
            </div>
            <EPGFileDialog
              selectedFile={epgFileDto}
              onEPGChanged={(e) => {
                setEPGFileDto(e);
              }}
              noButtons
            />
          </div>
        </SMCard>
      </Dialog>
      {/* </OverlayPanel> */}
      <div hidden={showButton === false} className="justify-content-center">
        <AddButton
          onClick={(e) => {
            op.current?.toggle(e);
            setVisible(true);
          }}
          tooltip="Add M3U File"
          iconFilled={false}
        />
      </div>
    </>
  );
};
EPGFileCreateDialog.displayName = 'EPGFileCreateDialog';

export default React.memo(EPGFileCreateDialog);
