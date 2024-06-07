import React, { useCallback, useRef } from 'react';

import { FileUpload } from 'primereact/fileupload';

import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import SMFileUpload from '@components/sm/SMFileUpload';

import { CreateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { CreateM3UFileRequest, M3UFileDto } from '@lib/smAPI/smapiTypes';
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
  const smDialogRef = useRef<SMDialogRef>(null);
  const defaultValues = {
    HoursToUpdate: 72,
    MaxStreamCount: 1,
    Name: '',
    OverwriteChannelNumbers: true,
    StartingChannelNumber: 1
  } as M3UFileDto;

  const [m3uFileDto, setM3UFileDto] = React.useState<M3UFileDto>(defaultValues);

  const ReturnToParent = useCallback(
    (didUpload?: boolean) => {
      if (fileUploadReference.current) {
        fileUploadReference.current.clear();
      }
      smDialogRef.current?.hide();
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
      createM3UFileRequest.OverWriteChannels = m3uFileDto.OverwriteChannelNumbers;
      createM3UFileRequest.VODTags = m3uFileDto.VODTags;
      createM3UFileRequest.HoursToUpdate = m3uFileDto.HoursToUpdate;

      await CreateM3UFile(createM3UFileRequest)
        .then(() => {})
        .catch((error) => {
          console.error('Error uploading M3U', error);
        })
        .finally(() => {
          smDialogRef.current?.hide();
        });
    },
    [m3uFileDto]
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
    <SMDialog
      ref={smDialogRef}
      widthSize={6}
      position="top-right"
      title="ADD M3U"
      onHide={() => ReturnToParent()}
      buttonClassName="icon-green-filled"
      tooltip="Add M3U"
      info="General"
    >
      <div className="w-12">
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
        <div className="layout-padding-bottom-lg" />
        <M3UFileDialog
          selectedFile={m3uFileDto}
          onM3UChanged={(e) => {
            setM3UFileDto(e);
          }}
          noButtons
        />
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMDialog>
  );
};
M3UFileCreateDialog.displayName = 'M3UFileCreateDialog';

export default React.memo(M3UFileCreateDialog);
