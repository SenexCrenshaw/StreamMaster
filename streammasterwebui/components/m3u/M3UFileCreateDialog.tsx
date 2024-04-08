import React, { useCallback, useRef } from 'react';

import AddButton from '@components/buttons/AddButton';
import { FileUpload } from 'primereact/fileupload';

import XButton from '@components/buttons/XButton';
import SMFileUpload from '@components/file/SMFileUpload';
import { CreateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { CreateM3UFileRequest, M3UFileDto } from '@lib/smAPI/smapiTypes';
import { OverlayPanel } from 'primereact/overlaypanel';
import M3UFileEditDialog from './M3UFileEditDialog';

export interface M3UFileDialogProperties {
  readonly infoMessage?: string;
  readonly onHide?: (didUpload: boolean) => void;
  readonly onUploadComplete: () => void;
  readonly show?: boolean | null;
  readonly showButton?: boolean | null;
}

export const M3UFileCreateDialog = ({ onHide, onUploadComplete, showButton }: M3UFileDialogProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const op = useRef<OverlayPanel>(null);

  const defaultValues = {
    Name: '',
    MaxStreamCount: 1,
    HoursToUpdate: 72,
    OverwriteChannelNumbers: true,
    StartingChannelNumber: 1
  } as M3UFileDto;

  const [m3uFileDto, setM3UFileDto] = React.useState<M3UFileDto>(defaultValues);

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
    async (name: string, source: string) => {
      const createM3UFileRequest = {} as CreateM3UFileRequest;

      createM3UFileRequest.Name = name;
      createM3UFileRequest.FormFile = undefined;
      createM3UFileRequest.UrlSource = source;
      createM3UFileRequest.MaxStreamCount = m3uFileDto.MaxStreamCount;
      createM3UFileRequest.StartingChannelNumber = m3uFileDto.StartingChannelNumber;
      createM3UFileRequest.VODTags = m3uFileDto.VODTags;

      await CreateM3UFile(createM3UFileRequest)
        .then(() => {
          //setInfoMessage('Uploaded M3U';
        })
        .catch((error) => {
          // setInfoMessage(`Error Uploading M3U: ${error.message}`);
        })
        .finally(() => {
          ReturnToParent(true);
        });
    },
    [ReturnToParent, m3uFileDto]
  );

  const setName = (value: string) => {
    if (m3uFileDto && m3uFileDto.Name !== value) {
      const m3uFileDtoCopy = { ...m3uFileDto };
      m3uFileDtoCopy.Name = value;
      console.log('M3UFileCreateDialog setName', value);
      setM3UFileDto(m3uFileDtoCopy);
    }
  };

  console.log('M3UFileCreateDialog', m3uFileDto);

  return (
    <>
      <OverlayPanel className="col-5 p-0 smfileupload-panel streammaster-border" ref={op} showCloseIcon={false} closeOnEscape>
        <div className="smfileupload col-12 p-0 m-0 ">
          <div className="smfileupload-header">
            <div className="flex justify-content-between align-items-center px-1 header">
              <span className="sm-text-color">ADD M3U FILE</span>
              <XButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} />
            </div>
          </div>
          <div className="px-2">
            <SMFileUpload
              m3uFileDto={m3uFileDto}
              onCreateFromSource={onCreateFromSource}
              onUploadComplete={() => {
                ReturnToParent(true);
              }}
              onName={(name) => {
                console.log('M3UFileCreateDialog SMFileUpload', name);
                setName(name);
              }}
            />
          </div>
          <M3UFileEditDialog
            selectedFile={m3uFileDto}
            onM3UChanged={(e) => {
              console.log('M3UFileCreateDialog', e.Name);
              setM3UFileDto(e);
            }}
            noButtons
          />
        </div>
      </OverlayPanel>

      <div hidden={showButton === false} className="justify-content-center">
        <AddButton onClick={(e) => op.current?.toggle(e)} tooltip="Add M3U File" />
      </div>
    </>
  );
};
M3UFileCreateDialog.displayName = 'M3UFileCreateDialog';

export default React.memo(M3UFileCreateDialog);
