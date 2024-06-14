import { UploadParamsSettings, useFileUpload } from '@components/file/useFileUpload';
import { useStringValue } from '@lib/redux/hooks/stringValue';
import { FileUpload } from 'primereact/fileupload';
import { forwardRef, memo, useImperativeHandle, useRef, useState } from 'react';
import SourceOrFileDialog from './SourceOrFileDialog';
import { SourceOrFileDialogProperties } from './interfaces/SourceOrFileDialogProperties';

type SMFileUploadProperties = UploadParamsSettings &
  SourceOrFileDialogProperties & {
    readonly onCreateFromSource: (source: string) => void;
    readonly onUploadComplete: () => void;
  };

export interface SMFileUploadRef {
  save: () => void;
  reset: () => void;
}

const SMFileUpload = forwardRef<SMFileUploadRef, SMFileUploadProperties>((props: SMFileUploadProperties, ref) => {
  const fileUploadReference = useRef<FileUpload>(null);
  const { stringValue } = useStringValue(props.m3uFileDto ? 'm3uName' : 'epgName');
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
      },
      save: () => {
        if (sourceOrFileDialogRef.current) {
          sourceOrFileDialogRef.current.save();
        }
      }
    }),
    []
  );

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
      ref={sourceOrFileDialogRef}
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
      {...props}
    />
  );
});

export default memo(SMFileUpload);
