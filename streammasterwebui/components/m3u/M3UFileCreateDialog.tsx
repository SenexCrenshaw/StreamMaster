import React, { useCallback, useRef, useState } from 'react';
import InfoMessageOverLayDialog from '@components/InfoMessageOverLayDialog';
import AddButton from '@components/buttons/AddButton';
import BooleanInput from '@components/inputs/BooleanInput';
import NumberInput from '@components/inputs/NumberInput';
import { FileUpload } from 'primereact/fileupload';
import M3UFileTags from './M3UFileTags';

import { useFileUpload } from '@components/sharedEPGM3U/useFileUpload';
import { CreateM3UFileRequest } from '@lib/smAPI/M3UFiles/M3UFilesTypes';
import { CreateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';

import SMFileUpload from '@components/file/SMFileUpload';
import { OverlayPanel } from 'primereact/overlaypanel';

export interface M3UFileDialogProperties {
  readonly infoMessage?: string;
  readonly onHide?: (didUpload: boolean) => void;
  readonly show?: boolean | null;
  readonly showButton?: boolean | null;
  readonly onUploadComplete: () => void;
}

export const M3UFileCreateDialog = ({ onHide, onUploadComplete, show, showButton }: M3UFileDialogProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const op = useRef<OverlayPanel>(null);

  const [overwriteChannelNumbers, setOverwriteChannelNumbers] = React.useState<boolean>(true);
  const [vodTags, setVodTags] = useState<string[]>([]);
  const [maxStreams, setMaxStreams] = useState<number>(1);
  const [startingChannelNumber, setStartingChannelNumber] = useState<number>(1);
  const [block, setBlock] = React.useState<boolean>(false);
  const { infoMessage, resetUploadState } = useFileUpload();
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);

  const ReturnToParent = (didUpload?: boolean) => {
    if (fileUploadReference.current) {
      fileUploadReference.current.clear();
    }
    resetUploadState();
    setShowOverlay(false);
    setBlock(false);

    setVodTags([]);

    setBlock(false);
    onHide?.(didUpload ?? false);
    onUploadComplete();
  };

  const onCreateFromSource = useCallback(async (name: string, source: string) => {
    const createM3UFileRequest = {} as CreateM3UFileRequest;

    createM3UFileRequest.name = name;
    createM3UFileRequest.formFile = undefined;
    createM3UFileRequest.urlSource = source;
    createM3UFileRequest.maxStreamCount = maxStreams;
    createM3UFileRequest.startingChannelNumber = startingChannelNumber;
    createM3UFileRequest.vODTags = vodTags;

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
  }, []);

  const settingTemplate = (): JSX.Element => {
    return (
      <div className="flex flex flex-wrap p-fluid align-items-center">
        <div className="col-2">
          <NumberInput
            label="Max Streams"
            onChange={(e) => {
              setMaxStreams(e);
            }}
            showClear
            value={maxStreams}
          />
        </div>

        <div className="col-2">
          <NumberInput
            label="Starting Channel #"
            onChange={(e) => {
              setStartingChannelNumber(e);
            }}
            showClear
            value={startingChannelNumber}
          />
        </div>
        <div className="col-4">
          <BooleanInput
            label="Autoset Channel #s"
            onChange={(e) => {
              setOverwriteChannelNumbers(e ?? true);
            }}
            checked={overwriteChannelNumbers}
          />
        </div>
        <div className="col-4">
          <M3UFileTags vodTags={vodTags} onChange={(e) => setVodTags(e)} />
        </div>
      </div>
    );
  };

  return (
    <>
      <OverlayPanel className="col-5 p-0" ref={op} showCloseIcon={false}>
        <div className="col-12 p-0 m-0 h-50rem">
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
