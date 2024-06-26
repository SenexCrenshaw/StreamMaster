import OKButton from '@components/buttons/OKButton';
import ResetButton from '@components/buttons/ResetButton';
import SMFileUpload, { SMFileUploadRef } from '@components/sm/SMFileUpload';
import SMPopUp from '@components/sm/SMPopUp';
import { getRandomColorHex } from '@lib/common/colors';
import { useStringValue } from '@lib/redux/hooks/stringValue';
import { CreateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { CreateEPGFileRequest, EPGFileDto } from '@lib/smAPI/smapiTypes';
import { FileUpload } from 'primereact/fileupload';
import React, { useCallback, useEffect, useRef } from 'react';
import EPGFileDialog, { EPGFileDialogRef } from './EPGFileDialog';

export interface EPGFileCreateDialogProperties {
  readonly onHide?: (didUpload: boolean) => void;
  readonly onUploadComplete: () => void;
  readonly showButton?: boolean | null;
}

export const EPGFileCreateDialog = ({ onHide, onUploadComplete, showButton }: EPGFileCreateDialogProperties) => {
  const { stringValue, setStringValue } = useStringValue('epgName');
  const fileUploadReference = useRef<FileUpload>(null);
  const smFileUploadRef = useRef<SMFileUploadRef>(null);
  const fileDialogRef = useRef<EPGFileDialogRef>(null);
  const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

  // eslint-disable-next-line react-hooks/exhaustive-deps
  const defaultValues = {
    Color: getRandomColorHex(),
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
        setStringValue('');
      }

      onHide?.(didUpload ?? false);
      onUploadComplete();
    },
    [onHide, onUploadComplete, setStringValue]
  );

  const onCreateFromSource = useCallback(
    async (source: string) => {
      const request = {} as CreateEPGFileRequest;

      request.Name = epgFileDto.Name;
      request.UrlSource = source;
      request.Color = epgFileDto.Color;
      request.EPGNumber = epgFileDto.EPGNumber;
      request.HoursToUpdate = epgFileDto.HoursToUpdate;
      request.TimeShift = epgFileDto.TimeShift;

      await CreateEPGFile(request)
        .then(() => {})
        .catch((error) => {
          console.error('Error uploading EPG', error);
        })
        .finally(() => {
          setStringValue('');
          ReturnToParent();
        });
    },
    [ReturnToParent, epgFileDto.Color, epgFileDto.EPGNumber, epgFileDto.HoursToUpdate, epgFileDto.Name, epgFileDto.TimeShift, setStringValue]
  );

  useEffect(() => {
    if (stringValue && epgFileDto.Name !== stringValue) {
      const m3uFileDtoCopy = { ...epgFileDto };
      m3uFileDtoCopy.Name = stringValue;
      setEPGFileDto(m3uFileDtoCopy);
    }
  }, [epgFileDto, stringValue]);

  return (
    <SMPopUp
      buttonClassName="icon-green"
      contentWidthSize="3"
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
              setEPGFileDto(defaultValues);
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
      title="Add EPG"
      zIndex={10}
    >
      <div className="layout-padding-bottom-lg" />
      <div className="w-12 px-2">
        <SMFileUpload
          isM3U={false}
          onSaveEnabled={(enabled) => {
            setIsSaveEnabled(enabled);
          }}
          ref={smFileUploadRef}
          epgFileDto={epgFileDto}
          onCreateFromSource={onCreateFromSource}
          onUploadComplete={() => {
            ReturnToParent(true);
          }}
        />
        <div className="layout-padding-bottom-lg" />
        <EPGFileDialog
          ref={fileDialogRef}
          selectedFile={epgFileDto}
          onEPGChanged={(e) => {
            setEPGFileDto(e);
          }}
        />
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMPopUp>
  );
};
EPGFileCreateDialog.displayName = 'EPGFileCreateDialog';

export default React.memo(EPGFileCreateDialog);
