import React, { useCallback, useRef, useState } from 'react';

import AddButton from '@components/buttons/AddButton';
import NumberInput from '@components/inputs/NumberInput';
import { FileUpload } from 'primereact/fileupload';
import M3UFileTags from './M3UFileTags';

import SMFileUpload from '@components/file/SMFileUpload';
import { CreateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { CreateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import { OverlayPanel } from 'primereact/overlaypanel';
import { ToggleButton } from 'primereact/togglebutton';

export interface M3UFileDialogProperties {
  readonly infoMessage?: string;
  readonly onHide?: (didUpload: boolean) => void;
  readonly onUploadComplete: () => void;
  readonly show?: boolean | null;
  readonly showButton?: boolean | null;
}

export const M3UFileCreateDialog = ({ onHide, onUploadComplete, show, showButton }: M3UFileDialogProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const op = useRef<OverlayPanel>(null);

  const [overwriteChannelNumbers, setOverwriteChannelNumbers] = React.useState<boolean>(true);
  const [vodTags, setVodTags] = useState<string[]>([]);
  const [maxStreams, setMaxStreams] = useState<number>(1);
  const [startingChannelNumber, setStartingChannelNumber] = useState<number>(1);
  const [autoUpdate, setAutoUpdate] = useState<number>(1);

  const ReturnToParent = useCallback(
    (didUpload?: boolean) => {
      if (fileUploadReference.current) {
        fileUploadReference.current.clear();
      }

      setVodTags([]);

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
      createM3UFileRequest.MaxStreamCount = maxStreams;
      createM3UFileRequest.StartingChannelNumber = startingChannelNumber;
      createM3UFileRequest.VODTags = vodTags;

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
    [ReturnToParent, maxStreams, startingChannelNumber, vodTags]
  );

  const rightSettingTemplate = (): JSX.Element => {
    return (
      <div className="flex flex-wrap p-fluid align-items-center justify-content-between">
        <div className="col-6">
          <NumberInput
            label="MAX STREAMS"
            onChange={(e) => {
              setMaxStreams(e);
            }}
            showClear
            value={maxStreams}
          />
        </div>
        <div className="col-6">
          <NumberInput
            label="AUTO UPDATE"
            onChange={(e) => {
              setAutoUpdate(e);
            }}
            showClear
            suffix=" Hours"
            value={autoUpdate}
          />
        </div>
      </div>
    );
  };

  const settingTemplate = (): JSX.Element => {
    return (
      <div className="flex flex-wrap p-fluid align-items-center justify-content-between">
        <div className="sourceOrFileDialog-toggle pb-4">
          <div className="flex flex-column">
            <div id="name" className="text-xs text-500 pb-1">
              AUTO SET CHANNEL #S:
            </div>
            <ToggleButton checked={overwriteChannelNumbers} onChange={(e) => setOverwriteChannelNumbers(e.value)} />
          </div>
        </div>

        <div className="col-3">
          <NumberInput
            label="STARTING CHANNEL #"
            onChange={(e) => {
              setStartingChannelNumber(e);
            }}
            showClear
            value={startingChannelNumber}
          />
        </div>

        <div className="col-6">
          <M3UFileTags vodTags={vodTags} onChange={(e) => setVodTags(e)} />
        </div>
      </div>
    );
  };

  return (
    <>
      <OverlayPanel className="col-5 p-0 smfileupload-panel streammaster-border" ref={op} showCloseIcon={false}>
        <div className="col-12 p-0 m-0">
          <SMFileUpload
            fileType="m3u"
            maxStreams={maxStreams}
            startingChannelNumber={startingChannelNumber}
            overwriteChannelNumbers={overwriteChannelNumbers}
            vodTags={vodTags}
            onCreateFromSource={onCreateFromSource}
            onUploadComplete={() => {
              ReturnToParent(true);
            }}
            settingTemplate={settingTemplate()}
            rightSettingTemplate={rightSettingTemplate()}
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
