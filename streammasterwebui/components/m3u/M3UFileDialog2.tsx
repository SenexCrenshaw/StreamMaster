import * as axios from 'axios';
import { M3UFileStreamUrlPrefix } from '@lib/common/streammaster_enums';
import { useM3UFilesCreateM3UFileMutation, type CreateM3UFileRequest } from '@lib/iptvApi';
import React, { useRef, useState } from 'react';
import InfoMessageOverLayDialog from '@components/InfoMessageOverLayDialog';
import AddButton from '@components/buttons/AddButton';
import BooleanInput from '@components/inputs/BooleanInput';
import NumberInput from '@components/inputs/NumberInput';
import TextInput from '@components/inputs/TextInput';
import { getColorHex } from '@lib/common/colors';
import { isValidUrl } from '@lib/common/common';
import { Accordion, AccordionTab } from 'primereact/accordion';
import { FileUpload, FileUploadHeaderTemplateOptions, FileUploadSelectEvent } from 'primereact/fileupload';
import M3UFileTags from './M3UFileTags';
import { upload } from '@lib/FileUploadService';
import { ProgressBar } from 'primereact/progressbar';
import { useFileUpload } from '@components/sharedEPGM3U/useFileUpload';

export interface M3UFileDialogProperties {
  readonly infoMessage?: string;
  readonly onHide?: (didUpload: boolean) => void;
  readonly show?: boolean | null;
  readonly showButton?: boolean | null;
}

const M3UFileDialog2 = ({ onHide, show, showButton }: M3UFileDialogProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const [streamURLPrefix, setStreamURLPrefix] = React.useState<M3UFileStreamUrlPrefix>(0);
  const [activeFile, setActiveFile] = useState<File | undefined>();
  const [name, setName] = useState<string>('');
  const [fileName, setFileName] = useState<string>('');
  const [overwriteChannelNumbers, setOverwriteChannelNumbers] = React.useState<boolean>(true);
  const [vodTags, setVodTags] = useState<string[]>([]);
  const [maxStreams, setMaxStreams] = useState<number>(1);
  const [epgNumber, setEpgNumber] = useState<number | undefined>(undefined);
  const [timeShift, setTimeShift] = useState<number | undefined>(undefined);
  const [color, setColor] = useState<string | undefined>(undefined);
  const [startingChannelNumber, setStartingChannelNumber] = useState<number>(1);
  // const [progress, setProgress] = useState<number>(0);
  const [source, setSource] = useState<string>('');
  const [uploadedBytes, setUploadedBytes] = useState<number>(0);
  // const [infoMessage, setInfoMessage] = useState<string | undefined>();
  const [activeIndex, setActiveIndex] = useState<number>(0);
  const [nameFromFileName, setNameFromFileName] = useState<boolean>(false);
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const { doUpload, progress, infoMessage, isUploading, resetUploadState } = useFileUpload();

  const [M3UFilesCreateM3UFileMutation] = useM3UFilesCreateM3UFileMutation();

  const ReturnToParent = (didUpload?: boolean) => {
    if (fileUploadReference.current) {
      fileUploadReference.current.clear();
    }
    resetUploadState();
    setShowOverlay(false);
    setBlock(false);
    // setInfoMessage(undefined);
    setStreamURLPrefix(0);
    // setProgress(0);
    setUploadedBytes(0);
    setName('');
    setVodTags([]);
    setNameFromFileName(false);
    setSource('');
    setActiveIndex(0);
    setFileName('');
    setEpgNumber(undefined);
    setTimeShift(undefined);
    setColor(undefined);
    setBlock(false);
    onHide?.(didUpload ?? false);
  };

  const onSetSource = (url: string | null) => {
    if (!url) {
      return;
    }

    setSource(url);
  };

  const nextStep = (index: number) => {
    if (index === null) index = 0;

    setActiveIndex(index);
  };

  const onCreateFromSource = async (name: string, source: string, maxStreams: number, startingChannelNumber: number, vodTags: string[]) => {
    const addM3UFileRequest = {} as CreateM3UFileRequest;

    addM3UFileRequest.name = name;
    addM3UFileRequest.formFile = null;
    addM3UFileRequest.urlSource = source;
    addM3UFileRequest.maxStreamCount = maxStreams;
    addM3UFileRequest.startingChannelNumber = startingChannelNumber;
    addM3UFileRequest.vodTags = vodTags;

    await M3UFilesCreateM3UFileMutation(addM3UFileRequest)
      .then(() => {
        // setInfoMessage('Uploaded M3U';
      })
      .catch((error) => {
        // setInfoMessage(`Error Uploading M3U: ${error.message}`);
      });
  };

  const startUpload = async () => {
    if (block) {
      ReturnToParent();
    }

    setBlock(true);

    if (source === '') {
      const meColor = color ?? getColorHex(epgNumber ?? 0);
      doUpload({
        name,
        source,
        fileName,
        maxStreams,
        epgNumber,
        timeShift,
        color: meColor,
        startingChannelNumber,
        overwriteChannelNumbers,
        vodTags,
        file: activeFile,
        fileType: 'm3u'
      });
    } else {
      onCreateFromSource?.(name, source, maxStreams, startingChannelNumber, vodTags);
    }
  };

  const emptyTemplate = () => (
    <div className="flex align-items-center justify-content-center">
      <i
        className="pi pi-file mt-3 p-5"
        style={{
          backgroundColor: 'var(--surface-b)',
          borderRadius: '50%',
          color: 'var(--surface-d)',
          fontSize: '5em'
        }}
      />
      <span className="my-5" style={{ color: 'var(--text-color-secondary)', fontSize: '1.2em' }}>
        Drag and Drop M3U Here
      </span>
    </div>
  );

  const chooseOptions = {
    className: 'p-button-rounded p-button-info',
    icon: 'pi pi-fw pi-plus',
    label: 'Add File'
  };

  const uploadOptions = {
    className: 'p-button-rounded p-button-success',
    icon: 'pi pi-fw pi-upload',
    label: 'Upload'
  };

  const cancelOptions = {
    className: 'p-button-rounded p-button-danger',
    icon: 'pi pi-fw pi-times',
    label: 'Remove File'
  };

  const onTemplateSelect = (e: FileUploadSelectEvent) => {
    setActiveFile(e.files[0]);

    if (name === '' || nameFromFileName) {
      setNameFromFileName(true);
      const parts = e.files[0].name.split('.');
      setFileName(e.files[0].name);
      setName(parts[0]);
    }
  };

  const valueTemplate = React.useMemo(() => {
    const formatedValue = fileUploadReference?.current ? fileUploadReference.current.formatSize(activeFile?.size ?? 0) : '0 B';

    const formatedUpload = fileUploadReference?.current ? fileUploadReference.current.formatSize(uploadedBytes) : '0 B';

    return (
      <span>
        {formatedUpload} /<b>{formatedValue}</b>
      </span>
    );
  }, [activeFile?.size, uploadedBytes]);

  const headerTemplate = (options: FileUploadHeaderTemplateOptions) => {
    const { chooseButton, uploadButton, cancelButton } = options;

    return (
      <div className="card card-container mb-3 ">
        <div className="card">
          <div className="flex justify-content-center flex-wrap card-container">
            <div className="flex align-items-center justify-content-center m-2">{chooseButton}</div>
            <div className="flex align-items-center justify-content-center m-2">{uploadButton}</div>
            <div className="flex align-items-center justify-content-center m-2">{cancelButton}</div>
          </div>
        </div>
        <div className="card">
          <div className="flex justify-content-center flex-wrap card-container">
            <div className="flex align-items-center justify-content-center m-2">
              {valueTemplate}
              <ProgressBar
                // displayValueTemplate={valueTemplate}
                style={{ height: '20px', width: '300px' }}
                value={progress}
              />
            </div>
          </div>
        </div>
      </div>
    );
  };

  const onTemplateClear = () => {
    // setProgress(0);
    resetUploadState();
    setActiveFile(undefined);
    setNameFromFileName(false);
  };

  const isSaveEnabled = React.useMemo((): boolean => {
    if (name === null || name === '' || source === null || source === '' || !isValidUrl(source)) {
      return false;
    }

    return true;
  }, [name, source]);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={`Add M3U File`}
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={8}
        show={showOverlay || show === true}
      >
        <div className="flex grid w-full justify-content-between align-items-center">
          <div className="flex col-12">
            <div className="flex col-4">
              <TextInput
                label="Name"
                onChange={(value) => {
                  setName(value);
                  setNameFromFileName(false);
                }}
                onResetClick={() => {
                  if (activeFile !== null && activeFile !== undefined) {
                    setNameFromFileName(true);
                    setName(activeFile.name.replace(/\.[^./]+$/, ''));
                  } else {
                    setName('');
                  }
                }}
                showClear
                value={name}
              />
            </div>

            <div className="flex col-8">
              <div className="flex col-3">
                <NumberInput
                  label="Max Streams"
                  onChange={(e) => {
                    setMaxStreams(e);
                  }}
                  showClear
                  value={maxStreams}
                />
              </div>

              <div className="flex col-3">
                <NumberInput
                  label="Starting Channel #"
                  onChange={(e) => {
                    setStartingChannelNumber(e);
                  }}
                  showClear
                  value={startingChannelNumber}
                />
              </div>
              <div className="flex col-3">
                <BooleanInput
                  label="Autoset Channel #s"
                  onChange={(e) => {
                    setOverwriteChannelNumbers(e ?? true);
                  }}
                  checked={overwriteChannelNumbers}
                />
              </div>
              <div className={`flex col-3`}>
                <M3UFileTags vodTags={vodTags} onChange={(e) => setVodTags(e)} />
              </div>
            </div>
          </div>
          <div className="flex col-12">
            <Accordion activeIndex={activeIndex} className="w-full" onTabChange={(e) => nextStep(e.index as number)}>
              <AccordionTab header="Add By URL">
                <div className="flex col-12">
                  <div className="flex col-8 mr-5">
                    <TextInput isUrl isValid={isValidUrl(source)} label="Source URL" onChange={onSetSource} placeHolder="http(s)://" showClear value={source} />
                  </div>
                  <div className="flex col-3 mt-2 justify-content-end">
                    <AddButton disabled={!isSaveEnabled} label={`Add M3U File`} onClick={async () => await startUpload()} tooltip="Add File" />
                  </div>
                </div>
              </AccordionTab>
              <AccordionTab header="Add By File">
                <div className="flex col-12 w-full justify-content-center align-items-center">
                  <FileUpload
                    cancelOptions={cancelOptions}
                    chooseOptions={chooseOptions}
                    className="w-full"
                    customUpload
                    emptyTemplate={emptyTemplate}
                    headerTemplate={headerTemplate}
                    maxFileSize={300_000_000}
                    onClear={onTemplateClear}
                    onError={onTemplateClear}
                    onRemove={() => setActiveFile(undefined)}
                    onSelect={onTemplateSelect}
                    ref={fileUploadReference}
                    style={{ width: '100vw' }}
                    uploadHandler={startUpload}
                    uploadOptions={uploadOptions}
                  />
                </div>
              </AccordionTab>
            </Accordion>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <div hidden={showButton === false}>
        <AddButton onClick={() => setShowOverlay(true)} tooltip="Add M3U File" />
      </div>
    </>
  );
};

M3UFileDialog2.displayName = 'M3UFileDialog2';

export default React.memo(M3UFileDialog2);
