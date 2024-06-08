import NumberEditor from '@components/inputs/NumberEditor';
import StringEditor from '@components/inputs/StringEditor';
import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import React, { forwardRef, useCallback, useEffect, useImperativeHandle, useMemo, useRef, useState } from 'react';

import BooleanEditor from '@components/inputs/BooleanEditor';
import { useStringValue } from '@lib/redux/hooks/stringValue';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import M3UFileTags from './M3UFileTags';

export interface M3UFileDialogProperties {
  readonly onHide?: (didUpload: boolean) => void;
  readonly selectedFile: M3UFileDto;
  readonly noButtons?: boolean;
  readonly onM3UChanged?: (m3uFileDto: M3UFileDto) => void;
  readonly onSaveEnabled?: (saveEnabled: boolean) => void;
}

export interface M3UFileDialogRef {
  reset: () => void;
  save: () => void;
}

const M3UFileDialog = forwardRef<M3UFileDialogRef, M3UFileDialogProperties>(({ onM3UChanged, onSaveEnabled, selectedFile, noButtons }, ref) => {
  const defaultValues = useMemo(
    () =>
      ({
        HoursToUpdate: 72,
        MaxStreamCount: 1,
        Name: '',
        OverwriteChannelNumbers: true,
        StartingChannelNumber: 1
      } as M3UFileDto),
    []
  );

  const { code } = useScrollAndKeyEvents();
  const divRef = useRef<HTMLDivElement>(null);

  const [m3uFileDto, setM3UFileDto] = useState<M3UFileDto>(defaultValues);
  const [originalM3UFileDto, setOriginalM3UFileDto] = useState<M3UFileDto | undefined>(undefined);
  const [request, setRequest] = useState<UpdateM3UFileRequest>({} as UpdateM3UFileRequest);
  const { setStringValue } = useStringValue('m3uName');

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
      reset: () => {
        if (originalM3UFileDto) {
          setM3UFileDto({ ...originalM3UFileDto });
        }
      },
      save: () => {
        onUpdated();
      }
    }),
    [onUpdated, originalM3UFileDto]
  );

  const isSaveEnabled = useMemo(() => {
    const isChanged = Object.keys(defaultValues).some((key) => m3uFileDto[key as keyof M3UFileDto] !== originalM3UFileDto?.[key as keyof M3UFileDto]);

    onSaveEnabled?.(isChanged);
    return isChanged;
  }, [defaultValues, onSaveEnabled, m3uFileDto, originalM3UFileDto]);

  const updateStateAndRequest = useCallback(
    (updatedFields: Partial<M3UFileDto>) => {
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

  return (
    <div ref={divRef}>
      <div className="w-12">
        <div className="flex gap-1">
          <div className="w-6">
            <StringEditor
              showClear
              disableDebounce
              darkBackGround
              autoFocus
              label="NAME"
              value={m3uFileDto?.Name}
              onChange={(e) => {
                updateStateAndRequest({ Name: e });
                setStringValue(e);
              }}
            />
          </div>
          <div className="w-6">
            <div className="flex gap-1">
              <div className="w-6 ">
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
                  label="START CHANNEL #"
                  onChange={(e) => updateStateAndRequest({ StartingChannelNumber: e })}
                  showButtons
                  value={m3uFileDto?.StartingChannelNumber}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className="layout-padding-bottom-lg" />
      {noButtons !== true && (
        <>
          <div className="w-12">
            <StringEditor showClear disableDebounce darkBackGround label="URL" value={m3uFileDto?.Url} onChange={(e) => updateStateAndRequest({ Url: e })} />
          </div>
          <div className="layout-padding-bottom-lg" />
        </>
      )}
      <div className="w-12">
        <div className="flex gap-1">
          <div className="flex w-6 gap-1">
            <div className="w-6">
              <div className="sm-sourceorfiledialog-toggle">
                <BooleanEditor
                  label="AUTO CH.#:"
                  checked={m3uFileDto?.OverwriteChannelNumbers}
                  onChange={(e) => updateStateAndRequest({ OverwriteChannelNumbers: e })}
                />
              </div>
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
          <div className="w-6">
            <M3UFileTags vodTags={m3uFileDto?.VODTags} onChange={(e) => updateStateAndRequest({ VODTags: e })} />
          </div>
        </div>
      </div>
    </div>
  );
});

M3UFileDialog.displayName = 'M3UFileDialog';

export default React.memo(M3UFileDialog);
