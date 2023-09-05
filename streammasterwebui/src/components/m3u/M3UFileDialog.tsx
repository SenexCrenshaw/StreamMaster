import * as axios from 'axios';
import { Accordion, AccordionTab } from 'primereact/accordion';
import { Button } from 'primereact/button';

import {
  FileUpload,
  type FileUploadHeaderTemplateOptions,
  type FileUploadSelectEvent,
} from 'primereact/fileupload';

import { InputText } from 'primereact/inputtext';
import { ProgressBar } from 'primereact/progressbar';
import React, { useRef, useState } from 'react';
import { upload } from '../../services/FileUploadService';
import { getTopToolOptions, isValidUrl } from '../../common/common';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import { type CreateM3UFileRequest, useM3UFilesCreateM3UFileMutation } from '../../store/iptvApi';
import { InputNumber } from 'primereact/inputnumber';
import AddButton from '../buttons/AddButton';


type M3UFileDialogProps = {
  onHide?: (didUpload: boolean) => void,
  show?: boolean | null,
  showButton?: boolean | null
};

const M3UFileDialog = (props: M3UFileDialogProps) => {

  const fileUploadRef = useRef<FileUpload>(null);

  const [activeIndex, setActiveIndex] = useState<number>(0);
  const [activeFile, setActiveFile] = useState<File | undefined>();
  const [name, setName] = useState<string>('');
  const [progress, setProgress] = useState<number>(0);
  const [source, setSource] = useState<string>('');
  const [uploadedBytes, setUploadedBytes] = useState<number>(0);
  const [maxStreamCount, setMaxStreamCount] = useState<number>(1);
  const [startingChannelNumber, setStartingChannelNumber] = useState<number>(1);

  const [nameFromFileName, setNameFromFileName] = useState<boolean>(false);
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');

  const [m3uFilesCreateM3UFileMutation] = useM3UFilesCreateM3UFileMutation();

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
      <div className="flex align-items-center flex-column">
        <i className="pi pi-file mt-3 p-5" style={{ backgroundColor: 'var(--surface-b)', borderRadius: '50%', color: 'var(--surface-d)', fontSize: '5em' }} />
        <span className="my-5" style={{ color: 'var(--text-color-secondary)', fontSize: '1.2em' }}>
          Drag and Drop Image Here
        </span>
      </div>
    );
  };

  const ReturnToParent = (didUpload?: boolean) => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);

    setActiveIndex(0);
    setProgress(0);
    setUploadedBytes(0);
    setName('');
    setNameFromFileName(false);
    setSource('');

    setBlock(false);
    props.onHide?.(didUpload ?? false);

  };

  const doUpload = async () => {
    setBlock(true);
    if (source !== '') {

      const createM3UFileRequest = {} as CreateM3UFileRequest;

      createM3UFileRequest.name = name;
      createM3UFileRequest.description = '';
      createM3UFileRequest.maxStreamCount = maxStreamCount;
      createM3UFileRequest.startingChannelNumber = startingChannelNumber;
      createM3UFileRequest.formFile = null;
      createM3UFileRequest.urlSource = source;

      await m3uFilesCreateM3UFileMutation(createM3UFileRequest)
        .then(() => {

          setInfoMessage(`Uploaded M3U: ${name}${activeFile ? '/' + activeFile.name : ''}`);

        }).catch((e) => {
          setInfoMessage(`Upload M3U: ${name}${activeFile ? '/' + activeFile.name : ''} Error: ${e.message}`);
        });

      // ReturnToParent(true);

    } else {
      try {
        await upload(
          name,
          source,
          name,
          activeFile,
          'm3u',
          (event: axios.AxiosProgressEvent) => {
            setUploadedBytes(event.loaded);
            const total = event.total !== undefined ? event.total : 1;
            const prog = Math.round(100 * event.loaded / total);

            setProgress(prog);
          },
        );
        setInfoMessage(`Uploaded M3U: ${name}${activeFile ? '/' + activeFile.name : ''}`);
        // ReturnToParent(true);
      } catch (error: axios.AxiosError | Error | unknown) {
        if (axios.isAxiosError(error)) {
          setInfoMessage(`Uploaded M3U: ${name}${activeFile ? '/' + activeFile.name : ''} Error: ${error.message}`);

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
    className: 'p-button-rounded min-h-3rem p-button-info',
    icon: 'pi pi-fw pi-plus',
    iconOnly: true,
  };

  const uploadOptions = {
    className: 'p-button-rounded min-h-3rem p-button-success',
    icon: 'pi pi-fw pi-upload',
    iconOnly: true,
  };

  const cancelOptions = {
    className: 'p-button-rounded min-h-3rem p-button-danger',
    icon: 'pi pi-fw pi-times',
    iconOnly: true,
  };

  const nextStep = (index: number) => {
    if (index === null) index = 0;

    setActiveIndex(index);
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
        header='Add M3U File'
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={6}
        show={showOverlay || props.show === true}
      >
        <div className="flex">
          <div className="field">
            <span className="p-input-icon-right p-float-label">
              <i
                className="pi pi-times-circle z-1"
                hidden={name === null || name === ''}
                onClick={() => {

                  if (activeFile !== null && activeFile !== undefined) {
                    setNameFromFileName(true);
                    setName(activeFile.name.replace(/\.[^/.]+$/, ''))
                  } else {
                    setName('')
                  }
                }

                }
              />
              <InputText
                autoFocus
                className='withpadding'
                id="name"
                onChange={(event) => {
                  setName(event.target.value)
                  setNameFromFileName(false);
                }
                }
                value={name}
              />
              <label htmlFor="name">Name</label>
            </span>
          </div>
          <div className="field ml-2">

            <InputNumber
              className="withpadding"
              locale="en-US"
              onChange={(e) => setMaxStreamCount(e.value as number)}
              prefix='Max '
              suffix=' streams'
              value={maxStreamCount}
            />

          </div>

          <div className="field ml-2" >

            <InputNumber
              className="withpadding"
              locale="en-US"
              onChange={(e) => setStartingChannelNumber(e.value as number)}
              prefix='Starting Ch #'
              value={startingChannelNumber}
            />

          </div>
        </div>
        <Accordion
          activeIndex={activeIndex}
          onTabChange={(e) => nextStep(e.index as number)}
        >
          <AccordionTab header={`${name} URL`}>
            <div className="flex">
              <div className="field w-10">
                <span className="p-input-icon-right p-float-label w-full">
                  <i
                    className="pi pi-times-circle"
                    hidden={source === null || source === ''}
                    onClick={() => setSource('')}
                  />
                  <InputText
                    className={`withpadding w-full ${isValidUrl(source) ? '' : 'p-invalid'}`}
                    id="sourceURL"
                    onChange={(event) => onSetSource(event.target.value)}
                    placeholder='https://'
                    value={source}
                  />
                  <label htmlFor="sourceURL">Source URL (://) </label>
                </span>
              </div>
              <div
                className="absolute right-0 mr-5"

              >
                <Button
                  disabled={!isSaveEnabled}
                  icon="pi pi-plus"
                  onClick={async () => await doUpload()}
                  rounded
                  severity="success"
                  size="small"
                  tooltip='Add M3U File'
                  tooltipOptions={getTopToolOptions}
                />
              </div>
            </div>
          </AccordionTab>
          <AccordionTab header={`${name} File`}>

            <FileUpload
              // itemTemplate={itemTemplate}
              // onUpload={onTemplateUpload}
              accept="xml"
              cancelOptions={cancelOptions}
              chooseOptions={chooseOptions}
              className=""
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

          </AccordionTab>
        </Accordion>

      </InfoMessageOverLayDialog >

      <div hidden={props.showButton === false}>
        <AddButton label='Add M3U File' onClick={() => setShowOverlay(true)} tooltip='Add M3U File' />

      </div>
    </>
  );
};

M3UFileDialog.displayName = 'M3UFileDialog';
M3UFileDialog.defaultProps = {
  showButton: true,
};

export default React.memo(M3UFileDialog);
