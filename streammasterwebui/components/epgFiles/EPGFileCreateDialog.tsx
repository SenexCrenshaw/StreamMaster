import React, { useCallback, useRef, useState } from 'react';

import AddButton from '@components/buttons/AddButton';
import { FileUpload } from 'primereact/fileupload';

import { SMCard } from '@components/sm/SMCard';
import XButton from '@components/buttons/XButton';
import SMFileUpload from '@components/file/SMFileUpload';

import { CreateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { CreateEPGFileRequest, EPGFileDto } from '@lib/smAPI/smapiTypes';
import { Dialog } from 'primereact/dialog';
import EPGFileDialog from './EPGFileDialog';
import { getRandomColorHex } from '@lib/common/colors';

export interface EPGFileCreateDialogProperties {
  readonly onHide?: (didUpload: boolean) => void;
  readonly onUploadComplete: () => void;
  readonly showButton?: boolean | null;
}

export const EPGFileCreateDialog = ({ onHide, onUploadComplete, showButton }: EPGFileCreateDialogProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const [visible, setVisible] = useState<boolean>(false);

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
      onUploadComplete();
    },
    [defaultValues, onHide, onUploadComplete]
  );

  const hide = useCallback(() => {
    setVisible(false);
    setEPGFileDto(defaultValues);
    onHide?.(false);
    defaultValues.Color = getRandomColorHex();
  }, [defaultValues, onHide]);

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
    <>
      <Dialog
        header="Header"
        visible={visible}
        style={{ width: '40vw' }}
        onHide={() => hide()}
        content={({ hide }) => (
          <SMCard title="ADD EPG" header={<XButton iconFilled={false} onClick={(e) => hide(e)} tooltip="Close" />}>
            <div className="sm-fileupload w-12 p-0 m-0 ">
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
        )}
      />
      <div hidden={showButton === false} className="justify-content-center">
        <AddButton
          onClick={(e) => {
            setVisible(true);
          }}
          tooltip="Add EPG File"
          iconFilled={false}
        />
      </div>
    </>
  );
};
EPGFileCreateDialog.displayName = 'EPGFileCreateDialog';

export default React.memo(EPGFileCreateDialog);
