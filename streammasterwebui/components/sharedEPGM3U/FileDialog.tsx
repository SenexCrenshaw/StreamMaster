import * as axios from 'axios';
import { FileUpload, type FileUploadHeaderTemplateOptions, type FileUploadSelectEvent } from 'primereact/fileupload';

import { ProgressBar } from 'primereact/progressbar';
import React, { useEffect, useRef, useState } from 'react';

import { M3UFileStreamUrlPrefix } from '@/lib/common/streammaster_enums';
import { upload } from '@lib/FileUploadService';
import { isValidUrl } from '@lib/common/common';
import { Accordion, AccordionTab } from 'primereact/accordion';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import AddButton from '../buttons/AddButton';
import NumberInput from '../inputs/NumberInput';
import TextInput from '../inputs/TextInput';
import StreamURLPrefixSelector from '../m3u/StreamURLPrefixSelector';

export type FileDialogProps = {
  readonly fileType: 'epg' | 'm3u';
  readonly infoMessage?: string;
  readonly onCreateFromSource?: (
    name: string,
    source: string,
    maxStreams: number,
    startingChannelNumber: number,
    streamURLPrefix: M3UFileStreamUrlPrefix,
  ) => void;
  readonly onHide?: (didUpload: boolean) => void;
  readonly show?: boolean | null;
  readonly showButton?: boolean | null;
};

const FileDialog: React.FC<FileDialogProps> = ({ fileType, infoMessage: inputInfoMessage, onCreateFromSource, onHide, show, showButton }) => {
  const labelName = fileType.toUpperCase();

  const fileUploadRef = useRef<FileUpload>(null);
  const [streamURLPrefix, setStreamURLPrefix] = React.useState<M3UFileStreamUrlPrefix>(0);
  const [activeFile, setActiveFile] = useState<File | undefined>();
  const [name, setName] = useState<string>('');
  const [maxStreams, setMaxStreams] = useState<number>(1);
  const [startingChannelNumber, setStartingChannelNumber] = useState<number>(1);
  const [progress, setProgress] = useState<number>(0);
  const [source, setSource] = useState<string>('');
  const [uploadedBytes, setUploadedBytes] = useState<number>(0);
  const [infoMessage, setInfoMessage] = useState<string | undefined>(undefined);
  const [activeIndex, setActiveIndex] = useState<number>(0);
  const [nameFromFileName, setNameFromFileName] = useState<boolean>(false);
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);

  useEffect(() => {
    setInfoMessage(inputInfoMessage);
  }, [inputInfoMessage]);

  const onTemplateSelect = (e: FileUploadSelectEvent) => {
    setActiveFile(e.files[0]);

    if (name === '' || nameFromFileName) {
      setNameFromFileName(true);
      setName(e.files[0].name.replace(/\.[^/.]+$/, ''));
    }
  };

  const onTemplateClear = () => {
    setProgress(0);
    setActiveFile(undefined);
    setNameFromFileName(false);
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

  const valueTemplate = React.useMemo(() => {
    const formatedValue = fileUploadRef?.current ? fileUploadRef.current.formatSize(activeFile?.size ?? 0) : '0 B';

    const formatedUpload = fileUploadRef?.current ? fileUploadRef.current.formatSize(uploadedBytes) : '0 B';

    return (
      <span>
        {formatedUpload} / <b>{formatedValue}</b>
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

  const emptyTemplate = () => {
    return (
      <div className="flex align-items-center justify-content-center">
        <i
          className="pi pi-file mt-3 p-5"
          style={{
            backgroundColor: 'var(--surface-b)',
            borderRadius: '50%',
            color: 'var(--surface-d)',
            fontSize: '5em',
          }}
        />
        <span className="my-5" style={{ color: 'var(--text-color-secondary)', fontSize: '1.2em' }}>
          {`Drag and Drop ${labelName} Here`}
        </span>
      </div>
    );
  };

  const ReturnToParent = (didUpload?: boolean) => {
    if (fileUploadRef.current) {
      fileUploadRef.current.clear();
    }
    setShowOverlay(false);
    setBlock(false);
    setInfoMessage(undefined);
    setStreamURLPrefix(0);
    setProgress(0);
    setUploadedBytes(0);
    setName('');
    setNameFromFileName(false);
    setSource('');
    setActiveIndex(0);
    setBlock(false);
    onHide?.(didUpload ?? false);
  };

  const doUpload = async () => {
    if (block) {
      ReturnToParent();
    }

    setBlock(true);

    if (source !== '') {
      onCreateFromSource?.(name, source, maxStreams, startingChannelNumber, streamURLPrefix);
    } else {
      await upload({
        name: name,
        source: source,
        description: name,
        streamURLPrefix: streamURLPrefix,
        file: activeFile,
        fileType: fileType,
        onUploadProgress: (event: axios.AxiosProgressEvent) => {
          setUploadedBytes(event.loaded);
          const total = event.total !== undefined ? event.total : 1;
          const prog = Math.round((100 * event.loaded) / total);
          setProgress(prog);
        },
      })
        .then(() => {
          setInfoMessage(`Uploaded ${labelName}`);
        })
        .catch((error: axios.AxiosError | Error | unknown) => {
          if (axios.isAxiosError(error)) {
            setInfoMessage(`Error Uploading ${labelName}: ${error.message}`);
          }
        });
    }
  };

  const chooseOptions = {
    className: 'p-button-rounded p-button-info',
    icon: 'pi pi-fw pi-plus',
    label: 'Add File',
  };

  const uploadOptions = {
    className: 'p-button-rounded p-button-success',
    icon: 'pi pi-fw pi-upload',
    label: 'Upload',
  };

  const cancelOptions = {
    className: 'p-button-rounded p-button-danger',
    icon: 'pi pi-fw pi-times',
    label: 'Remove File',
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
        header={`Add ${labelName} File`}
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={6}
        show={showOverlay || show === true}
      >
        <div className="flex grid w-full justify-content-between align-items-center">
          <div className="flex col-12">
            <div className={`flex col-${fileType === 'm3u' ? '4' : '12'}`}>
              <TextInput
                label="Name"
                onChange={(value) => {
                  setName(value);
                  setNameFromFileName(false);
                }}
                onResetClick={() => {
                  if (activeFile !== null && activeFile !== undefined) {
                    setNameFromFileName(true);
                    setName(activeFile.name.replace(/\.[^/.]+$/, ''));
                  } else {
                    setName('');
                  }
                }}
                showClear
                value={name}
              />
            </div>
            {fileType === 'm3u' && (
              <div className="flex col-8">
                <div className="flex col-4">
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
                    value={maxStreams}
                  />
                </div>
                <div className="flex col-5">
                  <StreamURLPrefixSelector
                    onChange={async (e) => {
                      setStreamURLPrefix(e);
                    }}
                    value={streamURLPrefix}
                  />
                </div>
              </div>
            )}
          </div>
          <div className="flex col-12">
            <Accordion activeIndex={activeIndex} className="w-full" onTabChange={(e) => nextStep(e.index as number)}>
              <AccordionTab header="Add By URL">
                <div className="flex col-12">
                  <div className="flex col-8 mr-5">
                    <TextInput isUrl isValid={isValidUrl(source)} label="Source URL" onChange={onSetSource} placeHolder="http(s)://" showClear value={source} />
                  </div>
                  <div className="flex col-3 mt-2 justify-content-end">
                    <AddButton disabled={!isSaveEnabled} label={`Add ${labelName} File`} onClick={async () => await doUpload()} tooltip="Add File" />
                  </div>
                </div>
              </AccordionTab>
              <AccordionTab header="Add By File">
                <div className="flex col-12 w-full justify-content-center align-items-center">
                  <FileUpload
                    // itemTemplate={itemTemplate}
                    // onUpload={onTemplateUpload}
                    //accept="xml"
                    cancelOptions={cancelOptions}
                    chooseOptions={chooseOptions}
                    className="w-full"
                    customUpload
                    emptyTemplate={emptyTemplate}
                    headerTemplate={headerTemplate}
                    maxFileSize={300000000}
                    onClear={onTemplateClear}
                    onError={onTemplateClear}
                    onRemove={() => setActiveFile(undefined)}
                    onSelect={onTemplateSelect}
                    ref={fileUploadRef}
                    style={{ width: '100vw' }}
                    uploadHandler={doUpload}
                    uploadOptions={uploadOptions}
                  />
                </div>
              </AccordionTab>
            </Accordion>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <div hidden={showButton === false}>
        <AddButton label={`Add ${labelName} File`} onClick={() => setShowOverlay(true)} tooltip={`Add ${labelName} File`} />
      </div>
    </>
  );
};

export default React.memo(FileDialog);