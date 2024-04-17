import React, { useCallback, useRef, useState } from 'react';

import AddButton from '@components/buttons/AddButton';
import { FileUpload } from 'primereact/fileupload';

import { SMCard } from '@components/sm/SMCard';
import XButton from '@components/buttons/XButton';
import SMFileUpload from '@components/file/SMFileUpload';
import { CreateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { CreateM3UFileRequest, M3UFileDto } from '@lib/smAPI/smapiTypes';
import { OverlayPanel } from 'primereact/overlaypanel';
import M3UFileDialog from './M3UFileDialog';
import { Dialog } from 'primereact/dialog';

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
  const [visible, setVisible] = useState<boolean>(false);

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
      <Dialog
        visible={visible}
        style={{ width: '40vw' }}
        onHide={() => setVisible(false)}
        content={({ hide }) => (
          <SMCard title="ADD M3U" header={<XButton iconFilled={false} onClick={(e) => setVisible(false)} tooltip="Close" />}>
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
        )}
      />
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
M3UFileCreateDialog.displayName = 'M3UFileCreateDialog';

export default React.memo(M3UFileCreateDialog);
