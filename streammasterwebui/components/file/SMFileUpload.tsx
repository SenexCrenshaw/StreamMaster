import { UploadParamsSettings, useFileUpload } from '@components/sharedEPGM3U/useFileUpload';
import { FileUpload } from 'primereact/fileupload';
import { InputText } from 'primereact/inputtext';
import { memo, useMemo, useRef, useState } from 'react';
import SourceOrFileDialog from './SourceOrFileDialog';

type SMFileUploadProperties = UploadParamsSettings & {
  readonly onCreateFromSource: (name: string, source: string) => void;
  readonly onUploadComplete: () => void;
  readonly rightSettingTemplate: JSX.Element;
  readonly settingTemplate: JSX.Element;
};

const SMFileUpload = (props: SMFileUploadProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);

  const { doUpload, progress, uploadedBytes, resetUploadState } = useFileUpload();
  const [block, setBlock] = useState<boolean>(false);
  const [name, setName] = useState<string>('');

  const ReturnToParent = (didUpload?: boolean) => {
    if (fileUploadReference.current) {
      fileUploadReference.current.clear();
    }
    resetUploadState();
    setName('');
    setBlock(false);
    props.onUploadComplete();
  };

  const startUpload = async (name: string, source: string | null, file: File | null) => {
    if (block) {
      ReturnToParent();
      return;
    }
    if (!source && !file) {
      ReturnToParent();
      return;
    }

    setBlock(true);

    await doUpload({
      ...props,
      file: file ?? undefined,
      name: name
    }).finally(() => {
      ReturnToParent(true);
    });
  };

  const getTemplate = useMemo(() => {
    if (!props.rightSettingTemplate) {
      return (
        <div className="col-12">
          <div className="flex flex-column">
            <div id="name" className="text-xs text-500">
              NAME:
            </div>
            <InputText className="p-float-label" id="name" value={name} onChange={(e) => setName(e.target.value)} />
          </div>
        </div>
      );
    }
    return (
      <div className="flex">
        <div className="col-6">
          <div id="name" className="text-xs text-500">
            NAME:
          </div>
          <InputText className="p-float-label w-full" id="name" value={name} onChange={(e) => setName(e.target.value)} />
        </div>
        <div className="col-6">{props.rightSettingTemplate}</div>
      </div>
    );
  }, [name, props.rightSettingTemplate]);

  return (
    <div>
      <div className="smfileupload-header ">
        <div className="flex justify-content-between align-items-center px-1 header border-round-md">
          <span className="sm-text-color">{`ADD ${props.fileType.toUpperCase()} FILE`}</span>
        </div>
      </div>
      <div className="smfileupload">
        <SourceOrFileDialog
          progress={progress}
          onAdd={(source, file) => {
            if (source) {
              console.log('Add from source ', source);
            } else if (file) {
              console.log('Add from file ', file.name);
              startUpload(name, source, file);
            } else {
              return;
            }
          }}
          onName={(name) => setName(name)}
        />
        <div className="col-12">{getTemplate}</div>
        <div className="col-12">{props.settingTemplate}</div>
      </div>
    </div>
  );
};

export default memo(SMFileUpload);
