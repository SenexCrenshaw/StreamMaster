import { UploadParamsSettings, useFileUpload } from '@components/file/useFileUpload';
import { FileUpload } from 'primereact/fileupload';
import { forwardRef, memo, useImperativeHandle, useRef, useState } from 'react';
import { SourceOrFileDialogProperties } from './Interfaces/SourceOrFileDialogProperties';
import SourceOrFileDialog from './SourceOrFileDialog';

type SMFileUploadProperties = UploadParamsSettings &
  SourceOrFileDialogProperties & {
    readonly onCreateFromSource: (source: string) => void;
    readonly onUploadComplete: () => void;
    readonly onFileNameChanged: (fileName: string) => void;
  };

export interface SMFileUploadRef {
  save: () => void;
  reset: () => void;
}

const SMFileUpload = forwardRef<SMFileUploadRef, SMFileUploadProperties>((props: SMFileUploadProperties, ref) => {
  const fileUploadReference = useRef<FileUpload>(null);
  // const { stringValue } = useStringValue(props.m3uFileDto ? 'm3uName' : 'epgName');
  const { doUpload, progress, resetUploadState } = useFileUpload();
  const [block, setBlock] = useState<boolean>(false);
  const sourceOrFileDialogRef = useRef<SMFileUploadRef>(null);

  useImperativeHandle(
    ref,
    () => ({
      reset: () => {
        if (sourceOrFileDialogRef.current) {
          sourceOrFileDialogRef.current.reset();
        }
        resetUploadState();

        sourceOrFileDialogRef.current?.reset();
      },
      save: () => {
        if (sourceOrFileDialogRef.current) {
          sourceOrFileDialogRef.current.save();
        }
      }
    }),
    [resetUploadState]
  );

  const ReturnToParent = (didUpload?: boolean) => {
    if (fileUploadReference.current) {
      fileUploadReference.current.clear();
    }
    resetUploadState();

    setBlock(false);
    sourceOrFileDialogRef.current?.reset();
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
      ref={sourceOrFileDialogRef}
      progress={progress}
      onFileNameChange={props.onFileNameChanged}
      onAdd={(source, file) => {
        if (source) {
          console.log('Add from source', source);
          props.onCreateFromSource(source);
          ReturnToParent();
        } else if (file && props.m3uFileDto?.Name) {
          console.log('Add M3U from file', file.name);
          startUpload(props.m3uFileDto?.Name, source, file);
        } else if (file && props.epgFileDto?.Name) {
          console.log('Add EPG from file', file.name);
          startUpload(props.epgFileDto?.Name, source, file);
        }
      }}
      {...props}
    />
  );
});

export default memo(SMFileUpload);
