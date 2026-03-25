import { Photo } from "./photo"

export interface Root {
  id: number
  username: string
  photoUrl: string
  age: number
  knownAs: string
  createdAt: Date
  lastActive: Date
  gender: string
  introduction: string
  interests: string
  city: string
  country: string
  photos: Photo[]
}
