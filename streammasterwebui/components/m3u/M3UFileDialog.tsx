import NumberEditor from '@components/inputs/NumberEditor';
import StringEditor from '@components/inputs/StringEditor';
import { arraysEqual } from '@components/smDataTable/helpers/arraysEqual';
import { Logger } from '@lib/common/logger';
import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { CreateM3UFile, UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { CreateM3UFileRequest, M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import React, { forwardRef, useCallback, useEffect, useImperativeHandle, useMemo, useRef, useState } from 'react';
import M3UFileTags from './M3UFileTags';
import SMFileUpload, { SMFileUploadRef } from '@components/sm/SMFileUpload';
import BooleanEditor from '@components/inputs/BooleanEditor';

export interface M3UFileDialogProperties {
  readonly onHide?: (didUpload: boolean) => void;
  readonly m3uFileDto: M3UFileDto | undefined;
  readonly showUrlEditor?: boolean;
  readonly onM3UChanged?: (m3uFileDto: M3UFileDto) => void;
  readonly onSaveEnabled?: (saveEnabled: boolean) => void;
}

export interface M3UFileDialogRef {
  hide: () => void;
  reset: () => void;
  save: () => void;
}

const M3UFileDialog = forwardRef<M3UFileDialogRef, M3UFileDialogProperties>(
  ({ onM3UChanged, onSaveEnabled, m3uFileDto: selectedFile, showUrlEditor = false }, ref) => {
    const smFileUploadRef = useRef<SMFileUploadRef>(null);
    const { code } = useScrollAndKeyEvents();
    const [m3uFileDto, setM3UFileDto] = useState<M3UFileDto | undefined>(undefined);
    const [originalM3UFileDto, setOriginalM3UFileDto] = useState<M3UFileDto | undefined>(undefined);
    const [request, setRequest] = useState<UpdateM3UFileRequest>({} as UpdateM3UFileRequest);
    // const { setStringValue } = useStringValue('m3uName');

    // const ReturnToParent = useCallback(
    //   (didUpload?: boolean) => {
    //     setStringValue('');
    //   },
    //   [setStringValue]
    // );
    const ReturnToParent = useCallback((didUpload?: boolean) => {}, []);

    const onUpdated = useCallback(async () => {
      if (request.Id === undefined) {
        return;
      }

      try {
        await UpdateM3UFile(request);
      } catch (error) {
        console.error(error);
      }
    }, [request]);

    useImperativeHandle(
      ref,
      () => ({
        hide: () => {
          setM3UFileDto(undefined);
          setOriginalM3UFileDto(undefined);
        },
        reset: () => {
          // if (originalM3UFileDto) {
          //   setM3UFileDto({ ...originalM3UFileDto });
          // } else {
          if (originalM3UFileDto) {
            setM3UFileDto({ ...originalM3UFileDto });
          }
          smFileUploadRef.current?.reset();
          // }
        },
        save: () => {
          if (showUrlEditor) {
            onUpdated();
          } else {
            smFileUploadRef.current?.save();
          }
        }
      }),
      []
    );

    const defaultValues = useMemo(
      () =>
        ({
          HoursToUpdate: 72,
          MaxStreamCount: 1,
          Name: '',
          SyncChannels: false,
          Url: ''
        } as M3UFileDto),
      []
    );

    const isSaveEnabled = useMemo(() => {
      if (m3uFileDto === undefined) {
        return false;
      }

      let isChanged = Object.keys(defaultValues).some((key) => m3uFileDto[key as keyof M3UFileDto] !== originalM3UFileDto?.[key as keyof M3UFileDto]);

      if (!isChanged && !arraysEqual(m3uFileDto.VODTags, originalM3UFileDto?.VODTags)) {
        isChanged = true;
      }

      onSaveEnabled?.(isChanged);
      return isChanged;
    }, [defaultValues, onSaveEnabled, m3uFileDto, originalM3UFileDto]);

    const onCreateFromSource = useCallback(
      async (source: string) => {
        if (!m3uFileDto) return;

        const createM3UFileRequest = {} as CreateM3UFileRequest;

        createM3UFileRequest.Name = m3uFileDto.Name;
        createM3UFileRequest.UrlSource = source;
        createM3UFileRequest.MaxStreamCount = m3uFileDto.MaxStreamCount;
        createM3UFileRequest.VODTags = m3uFileDto.VODTags;
        createM3UFileRequest.HoursToUpdate = m3uFileDto.HoursToUpdate;

        await CreateM3UFile(createM3UFileRequest)
          .then(() => {})
          .catch((error) => {
            console.error('Error uploading M3U', error);
          })
          .finally(() => {
            ReturnToParent();
          });
      },
      [ReturnToParent, m3uFileDto]
    );

    const updateStateAndRequest = useCallback(
      (updatedFields: Partial<M3UFileDto>) => {
        if (m3uFileDto === undefined) {
          return;
        }
        const updatedM3UFileDto = { ...m3uFileDto, ...updatedFields };
        const updatedRequest = { ...request, Id: updatedM3UFileDto.Id, ...updatedFields };

        setM3UFileDto(updatedM3UFileDto);
        setRequest(updatedRequest);

        if (onM3UChanged) onM3UChanged(updatedM3UFileDto);
      },
      [m3uFileDto, request, onM3UChanged]
    );

    useEffect(() => {
      if (selectedFile && (!originalM3UFileDto || selectedFile.Id !== originalM3UFileDto.Id)) {
        setM3UFileDto(selectedFile);
        setOriginalM3UFileDto(selectedFile);
      }
    }, [selectedFile, originalM3UFileDto]);

    useEffect(() => {
      if (code === 'Enter' || code === 'NumpadEnter') {
        if (isSaveEnabled) {
          onUpdated();
        }
      }
    }, [code, isSaveEnabled, onUpdated]);

    if (m3uFileDto !== undefined) {
      Logger.debug('M3UFileDialog', { isSaveEnabled, m3uFileDto: m3uFileDto.Url, originalM3UFileDto: originalM3UFileDto?.Url });
    }

    if (selectedFile === undefined) {
      return null;
    }

    Logger.debug('M3UFileDialog', 'm3uFileDto', m3uFileDto);

    if (m3uFileDto === undefined) {
      return null;
    }

    return (
      <div className="px-2">
        <div className="flex flex-wrap flex-row w-12">
          <div className="w-12">
            <SMFileUpload
              isM3U
              onSaveEnabled={(enabled) => {
                onSaveEnabled?.(enabled);
              }}
              ref={smFileUploadRef}
              m3uFileDto={m3uFileDto}
              onFileNameChanged={(e) => {
                if (!m3uFileDto || m3uFileDto.Name === '') updateStateAndRequest({ Name: e });
              }}
              onCreateFromSource={onCreateFromSource}
              onUploadComplete={() => {
                ReturnToParent(true);
              }}
            />
          </div>
          <div className="w-6">
            <StringEditor
              autoFocus
              darkBackGround
              disableDebounce
              label="NAME"
              onChange={(e) => {
                updateStateAndRequest({ Name: e });
                // setStringValue(e);
              }}
              value={m3uFileDto.Name}
            />
          </div>
          <div className="w-6">
            <div className="flex gap-1">
              <div className="w-6">
                <NumberEditor
                  darkBackGround
                  disableDebounce
                  label="MAX STREAMS"
                  onChange={(e) => updateStateAndRequest({ MaxStreamCount: e })}
                  showButtons
                  value={m3uFileDto?.MaxStreamCount}
                />
              </div>
              <div className="w-6">
                <NumberEditor
                  darkBackGround
                  disableDebounce
                  label="AUTO UPDATE"
                  onChange={(e) => updateStateAndRequest({ HoursToUpdate: e })}
                  showButtons
                  suffix=" Hours"
                  value={m3uFileDto?.HoursToUpdate}
                />
              </div>
            </div>
          </div>
        </div>

        <div className="layout-padding-bottom-lg" />
        {showUrlEditor === true && (
          <>
            <div className="w-12">
              <StringEditor disableDebounce darkBackGround label="URL" value={m3uFileDto?.Url} onChange={(e) => updateStateAndRequest({ Url: e })} />
            </div>
            <div className="layout-padding-bottom-lg" />
          </>
        )}
        <div className="w-12">
          <div className="flex gap-1">
            <div className="w-5">
              <BooleanEditor label="Sync Channels" onChange={(e) => updateStateAndRequest({ SyncChannels: e })} checked={m3uFileDto?.SyncChannels} />
            </div>
            <div className="w-7">
              <M3UFileTags vodTags={m3uFileDto?.VODTags} onChange={(e) => updateStateAndRequest({ VODTags: e })} />
            </div>
          </div>
          <div className="layout-padding-bottom-lg" />
        </div>
      </div>
    );
  }
);

M3UFileDialog.displayName = 'M3UFileDialog';

export default React.memo(M3UFileDialog);
