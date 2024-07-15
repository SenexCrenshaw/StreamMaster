import OKButton from '@components/buttons/OKButton';
import ResetButton from '@components/buttons/ResetButton';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { useStringValue } from '@lib/redux/hooks/stringValue';
import { FileUpload } from 'primereact/fileupload';
import React, { useCallback, useEffect, useMemo, useRef } from 'react';
import M3UFileDialog, { M3UFileDialogRef } from './M3UFileDialog';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';

export interface M3UFileDialogProperties {
  readonly infoMessage?: string;
  readonly onHide?: (didUpload: boolean) => void;
  readonly onUploadComplete: () => void;
  readonly show?: boolean | null;
  readonly showButton?: boolean | null;
}

export const M3UFileCreateDialog = ({ onHide, onUploadComplete, showButton }: M3UFileDialogProperties) => {
  const { stringValue } = useStringValue('m3uName');
  const fileUploadReference = useRef<FileUpload>(null);

  const dialogRef = useRef<SMPopUpRef>(null);
  const fileDialogRef = useRef<M3UFileDialogRef>(null);
  const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

  const defaultValues = useMemo(
    () =>
      ({
        HoursToUpdate: 72,
        MaxStreamCount: 1,
        Name: '',
        Url: ''
      } as M3UFileDto),
    []
  );

  const [m3uFileDto, setM3UFileDto] = React.useState<M3UFileDto>(defaultValues);

  const ReturnToParent = useCallback(
    (didUpload?: boolean) => {
      fileUploadReference.current?.clear();
      fileDialogRef.current?.hide();
      fileDialogRef.current?.reset();
      dialogRef.current?.hide();
      setM3UFileDto(defaultValues);
      onHide?.(didUpload ?? false);
      onUploadComplete();
    },
    [defaultValues, onHide, onUploadComplete]
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
      contentWidthSize="3"
      icon="pi-plus"
      zIndex={11}
      onCloseClick={() => {
        ReturnToParent();
      }}
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <ResetButton
            onClick={() => {
              // if (smFileUploadRef.current) {
              //   smFileUploadRef.current.reset();
              // }
              if (fileDialogRef.current) {
                fileDialogRef.current.reset();
              }
              setM3UFileDto(defaultValues);
            }}
          />
          <OKButton
            buttonDisabled={!isSaveEnabled}
            onClick={(request) => {
              fileDialogRef.current?.save();
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
    >
      <div className="layout-padding-bottom-lg" />
      <div className="sm-w-12">
        <M3UFileDialog
          ref={fileDialogRef}
          m3uFileDto={m3uFileDto}
          onM3UChanged={(e) => {
            setM3UFileDto(e);
          }}
          onSaveEnabled={(e) => {
            setIsSaveEnabled(e);
          }}
        />
      </div>
    </SMPopUp>
  );
};
M3UFileCreateDialog.displayName = 'M3UFileCreateDialog';

export default React.memo(M3UFileCreateDialog);
