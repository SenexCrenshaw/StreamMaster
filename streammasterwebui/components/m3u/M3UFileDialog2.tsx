import { useM3UFilesCreateM3UFileMutation } from '@lib/iptvApi';
import React, { useRef, useState } from 'react';
import InfoMessageOverLayDialog from '@components/InfoMessageOverLayDialog';
import AddButton from '@components/buttons/AddButton';
import BooleanInput from '@components/inputs/BooleanInput';
import NumberInput from '@components/inputs/NumberInput';
import TextInput from '@components/inputs/TextInput';

import { isValidUrl } from '@lib/common/common';
import { Accordion, AccordionTab } from 'primereact/accordion';
import { FileUpload, FileUploadHeaderTemplateOptions, FileUploadSelectEvent } from 'primereact/fileupload';
import M3UFileTags from './M3UFileTags';
import { ProgressBar } from 'primereact/progressbar';
import { useFileUpload } from '@components/sharedEPGM3U/useFileUpload';
import { CreateM3UFileRequest } from '@lib/smAPI/M3UFiles/M3UFilesTypes';
import { CreateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';

export interface M3UFileDialogProperties {
  readonly infoMessage?: string;
  readonly onHide?: (didUpload: boolean) => void;
  readonly show?: boolean | null;
  readonly showButton?: boolean | null;
}

const M3UFileDialog2 = ({ onHide, show, showButton }: M3UFileDialogProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const [activeFile, setActiveFile] = useState<File | undefined>();
  const [name, setName] = useState<string>('');
  const [fileName, setFileName] = useState<string>('');
  const [overwriteChannelNumbers, setOverwriteChannelNumbers] = React.useState<boolean>(true);
  const [vodTags, setVodTags] = useState<string[]>([]);
  const [maxStreams, setMaxStreams] = useState<number>(1);
  const [startingChannelNumber, setStartingChannelNumber] = useState<number>(1);
  const [source, setSource] = useState<string>('');
  const [activeIndex, setActiveIndex] = useState<number>(0);
  const [nameFromFileName, setNameFromFileName] = useState<boolean>(false);
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const { doUpload, progress, infoMessage, isUploading, uploadedBytes, resetUploadState } = useFileUpload();

  const [M3UFilesCreateM3UFileMutation] = useM3UFilesCreateM3UFileMutation();

  const ReturnToParent = (didUpload?: boolean) => {
    if (fileUploadReference.current) {
      fileUploadReference.current.clear();
    }
    resetUploadState();
    setShowOverlay(false);
    setBlock(false);
    setName('');
    setVodTags([]);
    setNameFromFileName(false);
    setSource('');
    setActiveIndex(0);
    setFileName('');

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
    const createM3UFileRequest = {} as CreateM3UFileRequest;

    createM3UFileRequest.name = name;
    createM3UFileRequest.formFile = undefined;
    createM3UFileRequest.urlSource = source;
    createM3UFileRequest.maxStreamCount = maxStreams;
    createM3UFileRequest.startingChannelNumber = startingChannelNumber;
    createM3UFileRequest.vODTags = vodTags;

    await CreateM3UFile(createM3UFileRequest)
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
      doUpload({
        name,
        source,
        fileName,
        maxStreams,
        epgNumber: undefined,
        timeShift: undefined,
        color: undefined,
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
