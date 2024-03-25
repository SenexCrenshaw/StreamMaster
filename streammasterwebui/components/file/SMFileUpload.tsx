import TextInput from '@components/inputs/TextInput';
import { UploadParamsSettings, useFileUpload } from '@components/sharedEPGM3U/useFileUpload';
import { FileUpload, FileUploadHeaderTemplateOptions, FileUploadSelectEvent } from 'primereact/fileupload';
import { memo, useRef, useState } from 'react';
import SourceOrFileDialog from './SourceOrFileDialog';

type SMFileUploadProperties = UploadParamsSettings & {
  readonly onCreateFromSource: (name: string, source: string) => void;
  readonly settingTemplate: JSX.Element;
  readonly onUploadComplete: () => void;
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
      name: name,
      file: file ?? undefined
    }).finally(() => {
      ReturnToParent(true);
    });
  };

  return (
    <div className="smfileupload">
      <div className="col-12">
        <TextInput
          label="Name"
          onChange={(value) => {
            setName(value);
          }}
          value={name}
        />
      </div>
      <div className="col-12">{props.settingTemplate}</div>
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
    </div>
  );
};

export default memo(SMFileUpload);
