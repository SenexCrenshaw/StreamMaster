import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetEPGFilePreviewByIdSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetEPGFilePreviewById } from './GetEPGFilePreviewByIdFetch';
import {FieldData, EPGFilePreviewDto,GetEPGFilePreviewByIdRequest } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<EPGFilePreviewDto[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetEPGFilePreviewById = (params?: GetEPGFilePreviewByIdRequest): Result => {
  const dispatch = useAppDispatch();
  const param = JSON.stringify(params);
  const data = useAppSelector((state) => state.GetEPGFilePreviewById.data[param]);
  const error = useAppSelector((state) => state.GetEPGFilePreviewById.error[param] ?? '');
  const isError = useAppSelector((state) => state.GetEPGFilePreviewById.isError[param] ?? false);
  const isForced = useAppSelector((state) => state.GetEPGFilePreviewById.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetEPGFilePreviewById.isLoading[param] ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch]
  );

  const SetIsLoading = useCallback(
    (isLoading: boolean, param: string): void => {
      dispatch(setIsLoading({ isLoading: isLoading, param: param }));
    },
    [dispatch]
  );

useEffect(() => {
  if (param === undefined) return;
  const state = store.getState().GetEPGFilePreviewById;

  if (data === undefined && state.isLoading[param] !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [SetIsForced, data, dispatch, param]);

useEffect(() => {
  const state = store.getState().GetEPGFilePreviewById;
  if (params === undefined || param === undefined && !isForced) return;
  if (state.isLoading[param]) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true, param);
  dispatch(fetchGetEPGFilePreviewById(params));
}, [data, dispatch, param, isForced, isLoading, SetIsLoading]);

const SetField = (fieldData: FieldData): void => {
  dispatch(setField({ fieldData: fieldData }));
};

const Clear = (): void => {
  dispatch(clear());
};

return {
  data,
  error,
  isError,
  isLoading,
  Clear,
  SetField,
  SetIsForced,
  SetIsLoading
};
};

export default useGetEPGFilePreviewById;
