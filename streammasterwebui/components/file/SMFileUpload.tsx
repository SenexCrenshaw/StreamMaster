import AddButton from '@components/buttons/AddButton';
import TextInput from '@components/inputs/TextInput';
import { cancelOptions, chooseOptions, emptyTemplate, uploadOptions } from '@components/m3u/DialogTemplates/DialogchooseOptions';
import { UploadParamsSettings, useFileUpload } from '@components/sharedEPGM3U/useFileUpload';
import { isValidUrl } from '@lib/common/common';
import { Accordion, AccordionTab } from 'primereact/accordion';
import { FileUpload, FileUploadHeaderTemplateOptions, FileUploadSelectEvent } from 'primereact/fileupload';
import { ProgressBar } from 'primereact/progressbar';
import { memo, useMemo, useRef, useState } from 'react';

type SMFileUploadProperties = UploadParamsSettings & {
  readonly onCreateFromSource: (name: string, source: string) => void;
  readonly settingTemplate: JSX.Element;
};

const SMFileUpload = (props: SMFileUploadProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const [file, setFile] = useState<File | undefined>();
  const { doUpload, progress, uploadedBytes, resetUploadState } = useFileUpload();
  const [block, setBlock] = useState<boolean>(false);

  const [activeIndex, setActiveIndex] = useState<number>(0);

  const [source, setSource] = useState<string>('');
  const [nameFromFileName, setNameFromFileName] = useState<boolean>(false);
  const [name, setName] = useState<string>('');
  const [fileName, setFileName] = useState<string>('');

  const ReturnToParent = (didUpload?: boolean) => {
    if (fileUploadReference.current) {
      fileUploadReference.current.clear();
    }
    setActiveIndex(0);
    setName('');
    setNameFromFileName(false);
    resetUploadState();
    setFileName('');
    setBlock(false);
    setSource('');
  };

  const startUpload = async () => {
    if (block) {
      ReturnToParent();
      return;
    }

    setBlock(true);

    doUpload({
      ...props,
      name: name,
      fileName: fileName,
      file: file
    });
  };

  const isSaveEnabled = useMemo((): boolean => {
    if (name === null || name === '' || source === null || source === '' || !isValidUrl(source)) {
      return false;
    }

    return true;
  }, [name, source]);

  const valueTemplate = useMemo(() => {
    const formatedValue = fileUploadReference?.current ? fileUploadReference.current.formatSize(file?.size ?? 0) : '0 B';
    const formatedUpload = fileUploadReference?.current ? fileUploadReference.current.formatSize(uploadedBytes) : '0 B';

    return (
      <span>
        {formatedUpload} /<b>{formatedValue}</b>
      </span>
    );
  }, [file?.size, uploadedBytes]);

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

  const onTemplateSelect = (e: FileUploadSelectEvent) => {
    setFile(e.files[0]);

    if (name === '' || nameFromFileName) {
      setNameFromFileName(true);
      const parts = e.files[0].name.split('.');
      setFileName(e.files[0].name);
      setName(parts[0]);
    }
  };

  const onTemplateClear = () => {
    resetUploadState();
    setFile(undefined);
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

  return (
    <div className="smfileupload">
      <div className="col-12">
        <TextInput
          label="Name"
          onChange={(value) => {
            setName(value);
            setNameFromFileName(false);
          }}
          onResetClick={() => {
            if (file !== null && file !== undefined) {
              setNameFromFileName(true);
              setName(file.name.replace(/\.[^./]+$/, ''));
            } else {
              setName('');
            }
          }}
          showClear
          value={name}
        />
      </div>
      <div className="col-12">{props.settingTemplate}</div>
      <Accordion activeIndex={activeIndex} onTabChange={(e) => nextStep(e.index as number)}>
        <AccordionTab header="Add By URL">
          <div className="flex justify-content-between align-items-center px-1">
            <div className="col-8 mr-5">
              <TextInput isUrl isValid={isValidUrl(source)} label="Source URL" onChange={onSetSource} placeHolder="http(s)://" showClear value={source} />
            </div>
            <div className="flex col-3 mt-2 ">
              <AddButton disabled={!isSaveEnabled} label={`Add M3U File`} onClick={async () => props.onCreateFromSource(name, source)} tooltip="Add File" />
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
              maxFileSize={300000000}
              onClear={onTemplateClear}
              onError={onTemplateClear}
              onRemove={() => setFile(undefined)}
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
  );
};

export default memo(SMFileUpload);
