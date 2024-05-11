import React, { useCallback, useRef } from 'react';

import { FileUpload } from 'primereact/fileupload';

import SMFileUpload from '@components/sm/SMFileUpload';

import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { getRandomColorHex } from '@lib/common/colors';
import { CreateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { CreateEPGFileRequest, EPGFileDto } from '@lib/smAPI/smapiTypes';
import EPGFileDialog from './EPGFileDialog';

export interface EPGFileCreateDialogProperties {
  readonly onHide?: (didUpload: boolean) => void;
  readonly onUploadComplete: () => void;
  readonly showButton?: boolean | null;
}

export const EPGFileCreateDialog = ({ onHide, onUploadComplete, showButton }: EPGFileCreateDialogProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const smDialogRef = useRef<SMDialogRef>(null);

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
      }
      setEPGFileDto(defaultValues);
      onHide?.(didUpload ?? false);
      smDialogRef.current?.close();
      onUploadComplete();
    },
    [defaultValues, onHide, onUploadComplete]
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
          ReturnToParent(true);
        });
    },
    [ReturnToParent, epgFileDto]
  );

  const setName = (value: string) => {
    if (epgFileDto && epgFileDto.Name !== value) {
      const epgFileDtoCopy = { ...epgFileDto };
      epgFileDtoCopy.Name = value;
      setEPGFileDto(epgFileDtoCopy);
    }
  };

  return (
    <SMDialog ref={smDialogRef} title="ADD EPG" onHide={() => ReturnToParent()} buttonClassName="icon-green-filled" tooltip="Add EPG" info="General">
      <div className="w-12">
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
        <div className="layout-padding-bottom-lg" />
        <EPGFileDialog
          selectedFile={epgFileDto}
          onEPGChanged={(e) => {
            setEPGFileDto(e);
          }}
          noButtons
        />
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMDialog>
  );
};
EPGFileCreateDialog.displayName = 'EPGFileCreateDialog';

export default React.memo(EPGFileCreateDialog);