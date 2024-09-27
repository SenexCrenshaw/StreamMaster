import { QueryHookResult } from '@lib/apiDefs';
import store, { RootState } from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, clearByTag, setField, setIsForced, setIsLoading } from './GetSMChannelChannelsSlice';
import { useCallback,useEffect } from 'react';
import { useSMContext } from '@lib/context/SMProvider';
import { SkipToken } from '@reduxjs/toolkit/query';
import { getParameters } from '@lib/common/getParameters';
import { fetchGetSMChannelChannels } from './GetSMChannelChannelsFetch';
import {FieldData, SMChannelDto,GetSMChannelChannelsRequest } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<SMChannelDto[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  ClearByTag: (tag: string) => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetSMChannelChannels = (params?: GetSMChannelChannelsRequest | undefined | SkipToken): Result => {
  const { isSystemReady } = useSMContext();
  const dispatch = useAppDispatch();
  const param = getParameters(params);
  const isForced = useAppSelector((state) => state.GetSMChannelChannels.isForced ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch]
  );
  const ClearByTag = useCallback(
    (tag: string): void => {
      dispatch(clearByTag({tag: tag }));
    },
    [dispatch]
  );



  const SetIsLoading = useCallback(
    (isLoading: boolean, param: string): void => {
      if (param === undefined) return;
      dispatch(setIsLoading({ isLoading: isLoading, param: param }));
    },
    [dispatch]
  );

const selectData = (state: RootState) => {
    if (param === undefined) return undefined;
    return state.GetSMChannelChannels.data[param] || undefined;
  };
const data = useAppSelector(selectData);

const selectError = (state: RootState) => {
    if (param === undefined) return undefined;
    return state.GetSMChannelChannels.error[param] || undefined;
  };
const error = useAppSelector(selectError);

const selectIsError = (state: RootState) => {
    if (param === undefined) return false;
    return state.GetSMChannelChannels.isError[param] || false;
  };
const isError = useAppSelector(selectIsError);

const selectIsLoading = (state: RootState) => {
    if (param === undefined) return false;
    return state.GetSMChannelChannels.isLoading[param] || false;
  };
const isLoading = useAppSelector(selectIsLoading);


useEffect(() => {
  if (param === undefined) return;
  const state = store.getState().GetSMChannelChannels;
  if (data === undefined && state.isLoading[param] !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [data, param, SetIsForced]);

useEffect(() => {
    if (!isSystemReady) return;
  if (param === undefined) return;
  const state = store.getState().GetSMChannelChannels;
  if (params === undefined || param === undefined || param === '{}' ) return;
  if (state.isLoading[param]) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true, param);
  dispatch(fetchGetSMChannelChannels(params as GetSMChannelChannelsRequest));
}, [SetIsLoading, data, dispatch, isForced, isSystemReady, param, params]);

const SetField = (fieldData: FieldData): void => {
  dispatch(setField({ fieldData: fieldData }));
};

const Clear = (): void => {
  dispatch(clear());
};

return {
  Clear,
  ClearByTag,
  data,
  error,
  isError,
  isLoading,
  SetField,
  SetIsForced,
  SetIsLoading
};
};

export default useGetSMChannelChannels;
