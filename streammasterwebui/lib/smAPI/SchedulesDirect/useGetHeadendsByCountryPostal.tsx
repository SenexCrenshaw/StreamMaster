import { QueryHookResult } from '@lib/apiDefs';
import store, { RootState } from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, clearByTag, setField, setIsForced, setIsLoading } from './GetHeadendsByCountryPostalSlice';
import { useCallback,useEffect } from 'react';
import { SkipToken } from '@reduxjs/toolkit/query';
import { getParameters } from '@lib/common/getParameters';
import { fetchGetHeadendsByCountryPostal } from './GetHeadendsByCountryPostalFetch';
import {FieldData, HeadendDto,GetHeadendsByCountryPostalRequest } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<HeadendDto[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  ClearByTag: (tag: string) => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetHeadendsByCountryPostal = (params?: GetHeadendsByCountryPostalRequest | undefined | SkipToken): Result => {
  const dispatch = useAppDispatch();
  const param = getParameters(params);
  const isForced = useAppSelector((state) => state.GetHeadendsByCountryPostal.isForced ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
    if (param === undefined) return;
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch, param]
  );
  const ClearByTag = useCallback(
    (tag: string): void => {
      dispatch(clearByTag({tag: tag }));
    },
    [dispatch, param]
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
    return state.GetHeadendsByCountryPostal.data[param] || undefined;
  };
const data = useAppSelector(selectData);

const selectError = (state: RootState) => {
    if (param === undefined) return undefined;
    return state.GetHeadendsByCountryPostal.error[param] || undefined;
  };
const error = useAppSelector(selectError);

const selectIsError = (state: RootState) => {
    if (param === undefined) return false;
    return state.GetHeadendsByCountryPostal.isError[param] || false;
  };
const isError = useAppSelector(selectIsError);

const selectIsLoading = (state: RootState) => {
    if (param === undefined) return false;
    return state.GetHeadendsByCountryPostal.isLoading[param] || false;
  };
const isLoading = useAppSelector(selectIsLoading);


useEffect(() => {
  if (param === undefined) return;
  const state = store.getState().GetHeadendsByCountryPostal;
  if (data === undefined && state.isLoading[param] !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [data, param, SetIsForced]);

useEffect(() => {
  if (param === undefined) return;
  const state = store.getState().GetHeadendsByCountryPostal;
  if (params === undefined || param === undefined || param === '{}' ) return;
  if (state.isLoading[param]) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true, param);
  dispatch(fetchGetHeadendsByCountryPostal(params as GetHeadendsByCountryPostalRequest));
}, [SetIsLoading, data, dispatch, isForced, param, params]);

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

export default useGetHeadendsByCountryPostal;
