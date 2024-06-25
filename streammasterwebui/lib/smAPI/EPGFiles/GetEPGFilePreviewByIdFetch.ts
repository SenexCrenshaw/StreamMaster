import { GetEPGFilePreviewById } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { GetEPGFilePreviewByIdRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetEPGFilePreviewById = createAsyncThunk('cache/getGetEPGFilePreviewById', async (param: GetEPGFilePreviewByIdRequest, thunkAPI) => {
  try {
    Logger.debug('Fetching GetEPGFilePreviewById');
    const response = await GetEPGFilePreviewById(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


