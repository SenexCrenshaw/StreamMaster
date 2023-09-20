import * as axios from 'axios';
import {
  FileUpload,
  type FileUploadHeaderTemplateOptions,
  type FileUploadSelectEvent,
} from 'primereact/fileupload';

import { ProgressBar } from 'primereact/progressbar';
import React, { useEffect, useRef, useState } from 'react';
import { isValidUrl } from '../../common/common';
import { upload } from '../../services/FileUploadService';

import { Accordion, AccordionTab } from 'primereact/accordion';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import AddButton from '../buttons/AddButton';
import NumberInput from '../inputs/NumberInput';
import TextInput from '../inputs/TextInput';


export type FileDialogProps = {
  readonly fileType: 'epg' | 'm3u';
  readonly infoMessage?: string,
  readonly onCreateFromSource?: (name: string, source: string, maxStreams: number) => void,
  readonly onHide?: (didUpload: boolean) => void,
  readonly show?: boolean | null,
  readonly showButton?: boolean | null
};

const FileDialog: React.FC<FileDialogProps> = ({ fileType, infoMessage: inputInfoMessage, onCreateFromSource, onHide, show, showButton }) => {
  const labelName = fileType.toUpperCase();

  const fileUploadRef = useRef<FileUpload>(null);

  const [activeFile, setActiveFile] = useState<File | undefined>();
  const [name, setName] = useState<string>('');
  const [maxStreams, setMaxStreams] = useState<number>(1);
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

    setSource(url)
  };

  const nextStep = (index: number) => {
    if (index === null) index = 0;

    setActiveIndex(index);
  };

  const valueTemplate = React.useMemo(() => {
    const formatedValue = fileUploadRef?.current
      ? fileUploadRef.current.formatSize(activeFile?.size ?? 0)
      : '0 B';

    const formatedUpload = fileUploadRef?.current
      ? fileUploadRef.current.formatSize(uploadedBytes)
      : '0 B';

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
            <div className="flex align-items-center justify-content-center m-2">
              {chooseButton}
            </div>
            <div className="flex align-items-center justify-content-center m-2">
              {uploadButton}
            </div>
            <div className="flex align-items-center justify-content-center m-2">
              {cancelButton}
            </div>
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
        <i className="pi pi-file mt-3 p-5" style={{ backgroundColor: 'var(--surface-b)', borderRadius: '50%', color: 'var(--surface-d)', fontSize: '5em' }} />
        <span className="my-5" style={{ color: 'var(--text-color-secondary)', fontSize: '1.2em' }}>
          {`Drag and Drop ${labelName} Here`}
        </span>
      </div>
    );
  };

  const ReturnToParent = (didUpload?: boolean) => {
    setShowOverlay(false);
    setBlock(false);

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
    setBlock(true);

    if (source !== '') {
      onCreateFromSource?.(name, source, 0);
    } else {
      try {
        await upload(
          name,
          source,
          name,
          activeFile,
          fileType,
          (event: axios.AxiosProgressEvent) => {
            setUploadedBytes(event.loaded);
            const total = event.total !== undefined ? event.total : 1;
            const prog = Math.round(100 * event.loaded / total);

            setProgress(prog);
          },
        );
        setInfoMessage(`Uploaded ${labelName}`);
        // ReturnToParent(true);
      } catch (error: axios.AxiosError | Error | unknown) {
        if (axios.isAxiosError(error)) {
          setInfoMessage(`Error Uploading ${labelName}: ${error.message}`);

          // if (error.response) {
          //   console.log(error.response.data);
          //   console.log(error.response.status);
          //   console.log(error.response.headers);
          // } else if (error.request) {
          //   console.log(error.request);
          // } else {
          //   console.log('Error', error.message);
          // }

          setUploadedBytes(0);
          setProgress(0);
        } else if (error instanceof Error) {

        }
      }

      setUploadedBytes(0);
      setProgress(0);
      setBlock(false);
    }
  };

  const chooseOptions = {
    className: 'p-button-rounded p-button-info',
    icon: 'pi pi-fw pi-plus',
    iconOnly: true,
  };

  const uploadOptions = {
    className: 'p-button-rounded p-button-success',
    icon: 'pi pi-fw pi-upload',
    iconOnly: true,
  };

  const cancelOptions = {
    className: 'p-button-rounded p-button-danger',
    icon: 'pi pi-fw pi-times',
    iconOnly: true,
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
        onClose={() => { ReturnToParent(); }}
        overlayColSize={4}
        show={showOverlay || show === true}
      >
        <div className="flex grid w-full justify-content-between align-items-center">
          <div className="flex col-12">
            <div className={`flex col-${fileType === 'm3u' ? '6' : '12'}`}>
              <TextInput
                label="Name"
                onChange={(value) => { setName(value); setNameFromFileName(false); }}
                onResetClick={() => {
                  if (activeFile !== null && activeFile !== undefined) {
                    setNameFromFileName(true);
                    setName(activeFile.name.replace(/\.[^/.]+$/, ''))
                  } else {
                    setName('')
                  }
                }}
                showClear
                value={name} />
            </div>
            {fileType === 'm3u' &&
              <div className="flex col-6">
                <NumberInput
                  label="Max Streams"
                  onChange={(e) => {
                    console.log(e);
                    setMaxStreams(e);
                  }
                  }
                  showClear
                  value={maxStreams}
                />
              </div>
            }
          </div>
          <div className="flex col-12">
            <Accordion activeIndex={activeIndex} className='w-full' onTabChange={(e) => nextStep(e.index as number)}>
              <AccordionTab header="Add By URL">
                <div className="flex col-12">
                  <div className="flex col-8 mr-5">
                    <TextInput
                      isUrl
                      isValid={isValidUrl(source)}
                      label="Source URL"
                      onChange={onSetSource}
                      placeHolder='http(s)://'
                      showClear
                      value={name} />
                  </div>
                  <div className="flex col-3 mt-2 justify-content-end">
                    <AddButton disabled={!isSaveEnabled} label={labelName} onClick={async () => await doUpload()} tooltip="Add File" />
                  </div>
                </div>
              </AccordionTab>
              <AccordionTab header="Add By File">

                <div className="flex col-12 justify-content-between align-items-center">
                  <FileUpload
                    // itemTemplate={itemTemplate}
                    // onUpload={onTemplateUpload}
                    accept="xml"
                    cancelOptions={cancelOptions}
                    chooseOptions={chooseOptions}
                    className='w-full'
                    customUpload
                    emptyTemplate={emptyTemplate}
                    headerTemplate={headerTemplate}
                    maxFileSize={300000000}
                    onClear={onTemplateClear}
                    onError={onTemplateClear}
                    onRemove={() => setActiveFile(undefined)}
                    onSelect={onTemplateSelect}
                    ref={fileUploadRef}
                    uploadHandler={doUpload}
                    uploadOptions={uploadOptions}
                  />
                </div>
              </AccordionTab>
            </Accordion>
          </div>
        </div>


      </InfoMessageOverLayDialog >

      <div hidden={showButton === false}>
        <AddButton label={`Add ${labelName} File`} onClick={() => setShowOverlay(true)} tooltip={`Add ${labelName} File`} />
      </div>
    </>
  );
};

FileDialog.displayName = 'FileDialog';
FileDialog.defaultProps = {
  showButton: true,
};

export default React.memo(FileDialog);
