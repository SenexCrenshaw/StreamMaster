import { UploadParamsSettings, useFileUpload } from '@components/sharedEPGM3U/useFileUpload';
import { useStringValue } from '@lib/redux/hooks/stringValue';
import { FileUpload } from 'primereact/fileupload';
import { memo, useRef, useState } from 'react';
import SourceOrFileDialog from './SourceOrFileDialog';

type SMFileUploadProperties = UploadParamsSettings & {
  readonly onCreateFromSource: (source: string) => void;
  readonly onUploadComplete: () => void;
  // onName(name: string): void;
};

const SMFileUpload = (props: SMFileUploadProperties) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const { stringValue } = useStringValue(props.m3uFileDto ? 'm3uName' : 'epgName');
  const { doUpload, progress, resetUploadState } = useFileUpload();
  const [block, setBlock] = useState<boolean>(false);

  const ReturnToParent = (didUpload?: boolean) => {
    if (fileUploadReference.current) {
      fileUploadReference.current.clear();
    }
    resetUploadState();

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

  return (
    <SourceOrFileDialog
      isM3U={props.m3uFileDto !== undefined}
      progress={progress}
      onAdd={(source, file) => {
        if (source) {
          console.log('Add from source', source);
          props.onCreateFromSource(source);
          ReturnToParent();
        } else if (file && stringValue) {
          console.log('Add from file', file.name);
          startUpload(stringValue, source, file);
        }
      }}
    />
  );
};

export default memo(SMFileUpload);
