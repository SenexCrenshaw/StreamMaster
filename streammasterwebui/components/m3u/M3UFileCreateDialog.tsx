import OKButton from '@components/buttons/OKButton';
import ResetButton from '@components/buttons/ResetButton';
import SMFileUpload, { SMFileUploadRef } from '@components/sm/SMFileUpload';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { useStringValue } from '@lib/redux/hooks/stringValue';
import { CreateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { CreateM3UFileRequest, M3UFileDto } from '@lib/smAPI/smapiTypes';
import { FileUpload } from 'primereact/fileupload';
import React, { useCallback, useEffect, useRef } from 'react';
import M3UFileDialog, { M3UFileDialogRef } from './M3UFileDialog';

export interface M3UFileDialogProperties {
  readonly infoMessage?: string;
  readonly onHide?: (didUpload: boolean) => void;
  readonly onUploadComplete: () => void;
  readonly show?: boolean | null;
  readonly showButton?: boolean | null;
}

export const M3UFileCreateDialog = ({ onHide, onUploadComplete, showButton }: M3UFileDialogProperties) => {
  const { stringValue, setStringValue } = useStringValue('m3uName');
  const fileUploadReference = useRef<FileUpload>(null);
  const smFileUploadRef = useRef<SMFileUploadRef>(null);
  const dialogRef = useRef<SMPopUpRef>(null);
  const fileDialogRef = useRef<M3UFileDialogRef>(null);
  const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

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
        setStringValue('');
      }

      onHide?.(didUpload ?? false);
      onUploadComplete();
    },
    [onHide, onUploadComplete, setStringValue]
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
          fileDialogRef.current?.hide();
          dialogRef.current?.hide();
          setStringValue('');
          ReturnToParent();
        });
    },
    [
      ReturnToParent,
      m3uFileDto.HoursToUpdate,
      m3uFileDto.MaxStreamCount,
      m3uFileDto.Name,
      m3uFileDto.OverwriteChannelNumbers,
      m3uFileDto.StartingChannelNumber,
      m3uFileDto.VODTags,
      setStringValue
    ]
  );

  useEffect(() => {
    if (stringValue && m3uFileDto.Name !== stringValue) {
      const m3uFileDtoCopy = { ...m3uFileDto };
      m3uFileDtoCopy.Name = stringValue;
      setM3UFileDto(m3uFileDtoCopy);
    }
  }, [m3uFileDto, stringValue]);

  return (
    <SMPopUp
      buttonClassName="icon-green"
      contentWidthSize="4"
      hasCloseButton={false}
      icon="pi-plus"
      onCloseClick={() => {
        ReturnToParent();
      }}
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <ResetButton
            onClick={() => {
              if (smFileUploadRef.current) {
                smFileUploadRef.current.reset();
              }
              if (fileDialogRef.current) {
                fileDialogRef.current.reset();
              }
              setM3UFileDto(defaultValues);
            }}
          />
          <OKButton
            buttonDisabled={!isSaveEnabled}
            onClick={(request) => {
              smFileUploadRef.current?.save();
              ReturnToParent();
            }}
          />
        </div>
      }
      iconFilled
      modal
      placement="bottom-end"
      ref={dialogRef}
      title="Add M3U"
      zIndex={10}
    >
      <div className="layout-padding-bottom-lg" />
      <div className="w-12">
        <SMFileUpload
          isM3U
          onSaveEnabled={(enabled) => {
            setIsSaveEnabled(enabled);
          }}
          ref={smFileUploadRef}
          m3uFileDto={m3uFileDto}
          onCreateFromSource={onCreateFromSource}
          onUploadComplete={() => {
            ReturnToParent(true);
          }}
        />
        <div className="layout-padding-bottom-lg" />
        <M3UFileDialog
          ref={fileDialogRef}
          selectedFile={m3uFileDto}
          onM3UChanged={(e) => {
            setM3UFileDto(e);
          }}
        />
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMPopUp>
  );
};
M3UFileCreateDialog.displayName = 'M3UFileCreateDialog';

export default React.memo(M3UFileCreateDialog);
