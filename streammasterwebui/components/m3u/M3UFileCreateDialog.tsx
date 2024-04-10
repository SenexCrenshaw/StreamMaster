import React, { useCallback, useRef } from 'react';

import AddButton from '@components/buttons/AddButton';
import { FileUpload } from 'primereact/fileupload';

import { SMCard } from '@components/SMCard';
import XButton from '@components/buttons/XButton';
import SMFileUpload from '@components/file/SMFileUpload';
import { CreateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { CreateM3UFileRequest, M3UFileDto } from '@lib/smAPI/smapiTypes';
import { OverlayPanel } from 'primereact/overlaypanel';
import M3UFileDialog from './M3UFileDialog';

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
    async (source: string) => {
      const createM3UFileRequest = {} as CreateM3UFileRequest;

      createM3UFileRequest.Name = m3uFileDto.Name;
      createM3UFileRequest.UrlSource = source;
      createM3UFileRequest.MaxStreamCount = m3uFileDto.MaxStreamCount;
      createM3UFileRequest.StartingChannelNumber = m3uFileDto.StartingChannelNumber;
      createM3UFileRequest.VODTags = m3uFileDto.VODTags;

      await CreateM3UFile(createM3UFileRequest)
        .then(() => {
          //setInfoMessage('Uploaded M3U';
        })
        .catch((error) => {
          console.error('Error uploading M3U', error);
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

  return (
    <>
      <OverlayPanel className="col-5 p-0 sm-fileupload-panel default-border" ref={op} showCloseIcon={false} closeOnEscape>
        <SMCard title="ADD M3U" header={<XButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} tooltip="Close" />}>
          <div className="sm-fileupload col-12 p-0 m-0 ">
            <div className="px-2">
              <SMFileUpload
                m3uFileDto={m3uFileDto}
                onCreateFromSource={onCreateFromSource}
                onUploadComplete={() => {
                  ReturnToParent(true);
                }}
                onName={(name) => {
                  setName(name);
                }}
              />
            </div>
            <M3UFileDialog
              selectedFile={m3uFileDto}
              onM3UChanged={(e) => {
                setM3UFileDto(e);
              }}
              noButtons
            />
          </div>
        </SMCard>
      </OverlayPanel>

      <div hidden={showButton === false} className="justify-content-center">
        <AddButton onClick={(e) => op.current?.toggle(e)} tooltip="Add M3U File" iconFilled={false} />
      </div>
    </>
  );
};
M3UFileCreateDialog.displayName = 'M3UFileCreateDialog';

export default React.memo(M3UFileCreateDialog);
