import ResetButton from '@components/buttons/ResetButton';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { getRandomColorHex } from '@lib/common/colors';
import { FileUpload } from 'primereact/fileupload';
import React, { useCallback, useRef } from 'react';
import EPGFileDialog, { EPGFileDialogRef } from './EPGFileDialog';
import { EPGFileDto } from '@lib/smAPI/smapiTypes';

export interface EPGFileCreateDialogProperties {
  readonly onHide?: (didUpload: boolean) => void;
  readonly onUploadComplete: () => void;
  readonly showButton?: boolean | null;
}

export const EPGFileCreateDialog = ({ onHide, onUploadComplete, showButton }: EPGFileCreateDialogProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
  const fileDialogRef = useRef<EPGFileDialogRef>(null);
  const dialogRef = useRef<SMPopUpRef>(null);

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
      fileUploadReference.current?.clear();
      // fileDialogRef.current?.hide();
      fileDialogRef.current?.reset();
      dialogRef.current?.hide();
      setEPGFileDto(defaultValues);
      onHide?.(didUpload ?? false);
      onUploadComplete();
    },
    [defaultValues, onHide, onUploadComplete]
  );

  // useEffect(() => {
  //   if (stringValue && epgFileDto.Name !== stringValue) {
  //     const m3uFileDtoCopy = { ...epgFileDto };
  //     m3uFileDtoCopy.Name = stringValue;
  //     setEPGFileDto(m3uFileDtoCopy);
  //   }
  // }, [epgFileDto, stringValue]);

  return (
    <SMPopUp
      buttonClassName="icon-green"
      contentWidthSize="3"
      icon="pi-plus"
      onOkClick={() => {
        fileDialogRef.current?.save();
        ReturnToParent();
      }}
      okButtonDisabled={!isSaveEnabled}
      onCloseClick={() => {
        ReturnToParent();
      }}
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <ResetButton
            onClick={() => {
              if (fileDialogRef.current) {
                fileDialogRef.current.reset();
              }
              setEPGFileDto(defaultValues);
            }}
          />
          {/* <OKButton
            buttonDisabled={!isSaveEnabled}
            onClick={(request) => {
              fileDialogRef.current?.save();
              ReturnToParent();
            }}
          /> */}
        </div>
      }
      iconFilled
      modal
      placement="bottom-end"
      title="Add EPG"
      zIndex={11}
      ref={dialogRef}
    >
      <div className="layout-padding-bottom-lg" />
      <div className="sm-w-12">
        <EPGFileDialog
          ref={fileDialogRef}
          selectedFile={epgFileDto}
          onEPGChanged={(e) => {
            setEPGFileDto(e);
          }}
          onSaveEnabled={(e) => {
            setIsSaveEnabled(e);
          }}
        />
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMPopUp>
  );
};
EPGFileCreateDialog.displayName = 'EPGFileCreateDialog';

export default React.memo(EPGFileCreateDialog);
